////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2022 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using PaintDotNet.Collections;
using PaintDotNet.Imaging;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using WebPFileType.Exif;
using WebPFileType.Properties;

namespace WebPFileType
{
    internal static class WebPFile
    {
        /// <summary>
        /// Gets the color profile from the WebP image.
        /// </summary>
        /// <param name="webpBytes">The WebP image data.</param>
        /// <returns>
        /// A byte array containing the color profile, if present; otherwise, <see langword="null"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        internal static byte[] GetColorProfileBytes(byte[] webpBytes)
        {
            return GetMetadataBytes(webpBytes, WebPNative.MetadataType.ColorProfile);
        }

        /// <summary>
        /// Gets the EXIF data from the WebP image.
        /// </summary>
        /// <param name="webpBytes">The WebP image data.</param>
        /// <returns>
        /// A byte array containing the EXIF data, if present; otherwise, <see langword="null"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        internal static byte[] GetExifBytes(byte[] webpBytes)
        {
            return GetMetadataBytes(webpBytes, WebPNative.MetadataType.EXIF);
        }

        /// <summary>
        /// Gets the XMP data from the WebP image.
        /// </summary>
        /// <param name="webpBytes">The WebP image data.</param>
        /// <returns>
        /// A byte array containing the XMP data, if present; otherwise, <see langword="null"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        internal static byte[] GetXmpBytes(byte[] webpBytes)
        {
            return GetMetadataBytes(webpBytes, WebPNative.MetadataType.XMP);
        }

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
        internal static unsafe Surface Load(byte[] webpBytes)
        {
            if (webpBytes == null)
            {
                throw new ArgumentNullException(nameof(webpBytes));
            }

            WebPNative.WebPGetImageInfo(webpBytes, out WebPNative.ImageInfo imageInfo);

            if (imageInfo.hasAnimation)
            {
                throw new WebPException(Resources.AnimatedWebPNotSupported);
            }

            Surface image = null;
            Surface temp = null;

            try
            {
                temp = new Surface(imageInfo.width, imageInfo.height);

                WebPNative.WebPLoad(webpBytes, temp);

                image = temp;
                temp = null;
            }
            finally
            {
                if (temp != null)
                {
                    temp.Dispose();
                }
            }

            return image;
        }

        /// <summary>
        /// The WebP save function.
        /// </summary>
        /// <param name="input">The input Document.</param>
        /// <param name="output">The output Stream.</param>
        /// <param name="quality">The WebP save quality.</param>
        /// <param name="preset">The WebP encoding preset.</param>
        /// <param name="scratchSurface">The scratch surface.</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <exception cref="FormatException">The image exceeds 16383 pixels in width and/or height.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory to save the image.</exception>
        /// <exception cref="WebPException">The encoder returned a non-memory related error.</exception>
        internal static void Save(
            Document input,
            Stream output,
            int quality,
            WebPPreset preset,
            Surface scratchSurface,
            ProgressEventHandler progressCallback)
        {
            if (input.Width > WebPNative.WebPMaxDimension || input.Height > WebPNative.WebPMaxDimension)
            {
                throw new FormatException(Resources.InvalidImageDimensions);
            }

            WebPNative.EncodeParams encParams = new()
            {
                quality = quality,
                preset = preset
            };

            scratchSurface.Clear();
            input.CreateRenderer().Render(scratchSurface);

            WebPNative.MetadataParams metadata = CreateWebPMetadata(input);

            WebPNative.WebPReportProgress encProgress = null;

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

            WebPNative.WebPSave(scratchSurface, output, encParams, metadata, encProgress);
        }

        private static WebPNative.MetadataParams CreateWebPMetadata(Document doc)
        {
            byte[] iccProfileBytes = null;
            byte[] exifBytes = null;
            byte[] xmpBytes = null;

            string colorProfile = doc.Metadata.GetUserValue(WebPMetadataNames.ColorProfile);
            if (!string.IsNullOrEmpty(colorProfile))
            {
                iccProfileBytes = Convert.FromBase64String(colorProfile);
            }

            string exif = doc.Metadata.GetUserValue(WebPMetadataNames.EXIF);
            if (!string.IsNullOrEmpty(exif))
            {
                exifBytes = Convert.FromBase64String(exif);
            }

            string xmp = doc.Metadata.GetUserValue(WebPMetadataNames.XMP);
            if (!string.IsNullOrEmpty(xmp))
            {
                xmpBytes = Convert.FromBase64String(xmp);
            }

            if (iccProfileBytes == null || exifBytes == null)
            {
                Dictionary<ExifPropertyPath, ExifValue> propertyItems = GetMetadataFromDocument(doc);

                if (propertyItems != null)
                {
                    ExifColorSpace exifColorSpace = ExifColorSpace.Srgb;

                    if (propertyItems.TryGetValue(ExifPropertyKeys.Photo.ColorSpace.Path, out ExifValue value))
                    {
                        propertyItems.Remove(ExifPropertyKeys.Photo.ColorSpace.Path);

                        if (MetadataHelpers.TryDecodeShort(value, out ushort colorSpace))
                        {
                            exifColorSpace = (ExifColorSpace)colorSpace;
                        }
                    }

                    const ExifColorSpace Uncalibrated = (ExifColorSpace)ushort.MaxValue;

                    if (iccProfileBytes != null)
                    {
                        exifColorSpace = Uncalibrated;
                    }
                    else
                    {
                        ExifPropertyPath iccProfileKey = ExifPropertyKeys.Image.InterColorProfile.Path;

                        if (propertyItems.TryGetValue(iccProfileKey, out ExifValue iccProfileItem))
                        {
                            iccProfileBytes = iccProfileItem.Data.ToArrayEx();
                            propertyItems.Remove(iccProfileKey);
                            exifColorSpace = Uncalibrated;
                        }
                    }

                    if (iccProfileBytes != null)
                    {
                        // Remove the InteroperabilityIndex and related tags, these tags should
                        // not be written if the image has an ICC color profile.
                        propertyItems.Remove(ExifPropertyKeys.Interop.InteroperabilityIndex.Path);
                        propertyItems.Remove(ExifPropertyKeys.Interop.InteroperabilityVersion.Path);
                    }

                    if (exifBytes == null)
                    {
                        exifBytes = new ExifWriter(doc, propertyItems, exifColorSpace).CreateExifBlob();
                    }
                }
            }

            if (xmpBytes == null)
            {
                XmpPacket xmpPacket = doc.Metadata.TryGetXmpPacket();

                if (xmpPacket != null)
                {
                    string xmpPacketAsString = xmpPacket.ToString();

                    xmpBytes = System.Text.Encoding.UTF8.GetBytes(xmpPacketAsString);
                }
            }

            if (iccProfileBytes != null || exifBytes != null || xmpBytes != null)
            {
                return new WebPNative.MetadataParams(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        private static Dictionary<ExifPropertyPath, ExifValue> GetMetadataFromDocument(Document doc)
        {
            Dictionary<ExifPropertyPath, ExifValue> items = null;

            Metadata metadata = doc.Metadata;
            ExifPropertyItem[] exifProperties = metadata.GetExifPropertyItems();

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

        /// <summary>
        /// Gets the metadata from the WebP image.
        /// </summary>
        /// <param name="webpBytes">The WebP image data.</param>
        /// <param name="type">The metadata type.</param>
        /// <returns>
        /// A byte array containing the requested metadata, if present; otherwise, <see langword="null"/>
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        private static byte[] GetMetadataBytes(byte[] webpBytes, WebPNative.MetadataType type)
        {
            if (webpBytes == null)
            {
                throw new ArgumentNullException(nameof(webpBytes));
            }

            byte[] bytes = null;

            uint size = WebPNative.GetMetadataSize(webpBytes, type);
            if (size > 0)
            {
                bytes = new byte[size];
                WebPNative.ExtractMetadata(webpBytes, type, bytes, size);
            }

            return bytes;
        }
    }
}
