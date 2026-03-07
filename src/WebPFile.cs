////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using PaintDotNet.FileTypes;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebPFileType.Exif;
using WebPFileType.Interop;
using WebPFileType.Properties;

using ExifColorSpace = WebPFileType.Exif.ExifColorSpace;

namespace WebPFileType
{
    internal static class WebPFile
    {
        /// <summary>
        /// The WebP load function.
        /// </summary>
        /// <param name="webpBytes">The input image data</param>
        /// <returns>
        /// A <see cref="Bitmap"/> containing the WebP image.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory to load the WebP image.</exception>
        /// <exception cref="WebPException">
        /// The WebP image is invalid.
        /// -or-
        /// A native API parameter is invalid.
        /// </exception>
        internal static (IBitmap<ColorBgra32>, DecoderMetadata) Load(byte[] webpBytes) => WebPNative.WebPLoad(webpBytes);

        /// <summary>
        /// The WebP save function.
        /// </summary>
        /// <param name="input">The input Document.</param>
        /// <param name="output">The output Stream.</param>
        /// <param name="quality">The WebP save quality.</param>
        /// <param name="effort">The WebP encoding effort.</param>
        /// <param name="preset">The WebP encoding preset.</param>
        /// <param name="lossless">
        /// <see langword="true"/> if lossless encoding should be used; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <exception cref="FormatException">The image exceeds 16383 pixels in width and/or height.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory to save the image.</exception>
        /// <exception cref="WebPException">The encoder returned a non-memory related error.</exception>
        internal static void Save(
            IReadOnlyFileTypeDocument input,
            Stream output,
            int quality,
            int effort,
            WebPPreset preset,
            bool lossless,
            ProgressEventHandler progressCallback,
            IImagingFactory imagingFactory)
        {
            if (input.Size.Width > WebPNative.WebPMaxDimension || input.Size.Height > WebPNative.WebPMaxDimension)
            {
                throw new FormatException(Resources.InvalidImageDimensions);
            }

            EncoderOptions options = new()
            {
                quality = quality,
                effort = effort,
                preset = preset,
                lossless = lossless
            };

            using IFileTypeCompositeBitmap<ColorBgra32> docComposite = input.GetCompositeBitmap<ColorBgra32>();
            using IFileTypeBitmapLock<ColorBgra32> docCompositeLock = docComposite.Lock();

            EncoderMetadata? metadata = CreateWebPMetadata(input.Size, input.Metadata);

            WebPReportProgress? encProgress = null;

            if (progressCallback != null)
            {
                encProgress = delegate(int percent)
                {
                    try
                    {
                        progressCallback(null, new ProgressEventArgs(percent, true));
                        return true;
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                };
            }

            WebPNative.WebPSave(docCompositeLock.AsRegionPtr(), output, options, metadata, encProgress);
        }

        private static EncoderMetadata? CreateWebPMetadata(SizeInt32 docSize, IReadOnlyFileTypeDocumentMetadata metadata)
        {
            byte[]? iccProfileBytes = null;
            byte[]? exifBytes = null;
            byte[]? xmpBytes = null;

            metadata.Custom.TryGetValue(WebPMetadataNames.ColorProfile, out string? colorProfile);
            if (!string.IsNullOrEmpty(colorProfile))
            {
                iccProfileBytes = Convert.FromBase64String(colorProfile);
            }

            metadata.Custom.TryGetValue(WebPMetadataNames.EXIF, out string? exif);
            if (!string.IsNullOrEmpty(exif))
            {
                exifBytes = Convert.FromBase64String(exif);
            }

            metadata.Custom.TryGetValue(WebPMetadataNames.XMP, out string? xmp);
            if (!string.IsNullOrEmpty(xmp))
            {
                xmpBytes = Convert.FromBase64String(xmp);
            }

            if (iccProfileBytes == null || exifBytes == null)
            {
                Dictionary<ExifPropertyPath, ExifValue>? propertyItems = GetExifMetadata(metadata.Exif);

                if (propertyItems != null)
                {
                    ExifColorSpace exifColorSpace = ExifColorSpace.Srgb;

                    if (propertyItems.TryGetValue(ExifPropertyKeys.Photo.ColorSpace.Path, out ExifValue? value))
                    {
                        propertyItems.Remove(ExifPropertyKeys.Photo.ColorSpace.Path);

                        if (MetadataHelpers.TryDecodeShort(value, out ushort colorSpace))
                        {
                            exifColorSpace = (ExifColorSpace)colorSpace;
                        }
                    }

                    if (iccProfileBytes != null)
                    {
                        exifColorSpace = ExifColorSpace.Uncalibrated;
                    }
                    else
                    {
                        ExifPropertyPath iccProfileKey = ExifPropertyKeys.Image.InterColorProfile.Path;

                        if (propertyItems.TryGetValue(iccProfileKey, out ExifValue? iccProfileItem))
                        {
                            iccProfileBytes = [.. iccProfileItem.Data];
                            propertyItems.Remove(iccProfileKey);
                            exifColorSpace = ExifColorSpace.Uncalibrated;
                        }
                    }

                    if (iccProfileBytes != null)
                    {
                        // Remove the InteroperabilityIndex and related tags, these tags should
                        // not be written if the image has an ICC color profile.
                        propertyItems.Remove(ExifPropertyKeys.Interop.InteroperabilityIndex.Path);
                        propertyItems.Remove(ExifPropertyKeys.Interop.InteroperabilityVersion.Path);
                    }

                    exifBytes ??= new ExifWriter(docSize, propertyItems, exifColorSpace).CreateExifBlob();
                }
            }

            if (xmpBytes == null)
            {
                XmpPacket? xmpPacket = metadata.Xmp.XmpPacket;

                if (xmpPacket != null)
                {
                    string xmpPacketAsString = xmpPacket.ToString();

                    xmpBytes = System.Text.Encoding.UTF8.GetBytes(xmpPacketAsString);
                }
            }

            if (iccProfileBytes != null || exifBytes != null || xmpBytes != null)
            {
                return new EncoderMetadata(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        private static Dictionary<ExifPropertyPath, ExifValue>? GetExifMetadata(IReadOnlyFileTypeExifMetadata exifMetadata)
        {
            Dictionary<ExifPropertyPath, ExifValue>? items = null;

            ExifPropertyItem[] exifProperties = exifMetadata.Items.ToArray();

            if (exifProperties.Length > 0)
            {
                items = new Dictionary<ExifPropertyPath, ExifValue>(exifProperties.Length);

                foreach (ExifPropertyItem property in exifProperties)
                {
                    items.TryAdd(property.Path, property.Value);
                }
            }

            return items;
        }
    }
}
