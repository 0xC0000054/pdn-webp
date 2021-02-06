////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2021 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        internal static unsafe Bitmap Load(byte[] webpBytes)
        {
            if (webpBytes == null)
            {
                throw new ArgumentNullException(nameof(webpBytes));
            }

            WebPNative.ImageInfo imageInfo;

            WebPNative.WebPGetImageInfo(webpBytes, out imageInfo);

            if (imageInfo.hasAnimation)
            {
                throw new WebPException(Resources.AnimatedWebPNotSupported);
            }

            Bitmap image = null;
            Bitmap temp = null;

            try
            {
                temp = new Bitmap(imageInfo.width, imageInfo.height, PixelFormat.Format32bppArgb);

                BitmapData bitmapData = temp.LockBits(new Rectangle(0, 0, imageInfo.width, imageInfo.height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    WebPNative.WebPLoad(webpBytes, bitmapData);
                }
                finally
                {
                    temp.UnlockBits(bitmapData);
                }

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

            WebPNative.EncodeParams encParams = new WebPNative.EncodeParams
            {
                quality = quality,
                preset = preset
            };

            using (RenderArgs ra = new RenderArgs(scratchSurface))
            {
                input.Render(ra, true);
            }

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
                Dictionary<MetadataKey, MetadataEntry> propertyItems = GetMetadataFromDocument(doc);

                if (propertyItems != null)
                {
                    ExifColorSpace exifColorSpace = ExifColorSpace.Srgb;

                    if (iccProfileBytes != null)
                    {
                        exifColorSpace = ExifColorSpace.Uncalibrated;
                    }
                    else
                    {
                        MetadataKey iccProfileKey = MetadataKeys.Image.InterColorProfile;

                        if (propertyItems.TryGetValue(iccProfileKey, out MetadataEntry iccProfileItem))
                        {
                            iccProfileBytes = iccProfileItem.GetData();
                            propertyItems.Remove(iccProfileKey);
                            exifColorSpace = ExifColorSpace.Uncalibrated;
                        }
                    }

                    if (exifBytes == null)
                    {
                        exifBytes = new ExifWriter(doc, propertyItems, exifColorSpace).CreateExifBlob();
                    }
                }
            }

#if !PDN_3_5_X
            if (xmpBytes == null)
            {
                PaintDotNet.Imaging.XmpPacket xmpPacket = doc.Metadata.TryGetXmpPacket();

                if (xmpPacket != null)
                {
                    string xmpPacketAsString = xmpPacket.ToString();

                    xmpBytes = System.Text.Encoding.UTF8.GetBytes(xmpPacketAsString);
                }
            }
#endif

            if (iccProfileBytes != null || exifBytes != null || xmpBytes != null)
            {
                return new WebPNative.MetadataParams(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        private static Dictionary<MetadataKey, MetadataEntry> GetMetadataFromDocument(Document doc)
        {
            Dictionary<MetadataKey, MetadataEntry> items = null;

            Metadata metadata = doc.Metadata;

#if PDN_3_5_X
            string[] exifKeys = metadata.GetKeys(Metadata.ExifSectionName);

            if (exifKeys.Length > 0)
            {
                items = new Dictionary<MetadataKey, MetadataEntry>(exifKeys.Length);

                foreach (string key in exifKeys)
                {
                    string blob = metadata.GetValue(Metadata.ExifSectionName, key);
                    PropertyItem pi = null;

                    try
                    {
                        pi = PaintDotNet.SystemLayer.PdnGraphics.DeserializePropertyItem(blob);
                    }
                    catch
                    {
                        // Ignore any items that cannot be deserialized.
                    }

                    if (pi != null)
                    {
                        MetadataKey metadataKey = new MetadataKey(ExifTagHelper.GuessTagSection(pi), (ushort)pi.Id);

                        if (!items.ContainsKey(metadataKey))
                        {
                            items.Add(metadataKey, new MetadataEntry(metadataKey, (TagDataType)pi.Type, pi.Value));
                        }
                    }
                }
            }
#else
            PaintDotNet.Imaging.ExifPropertyItem[] exifProperties = metadata.GetExifPropertyItems();

            if (exifProperties.Length > 0)
            {
                items = new Dictionary<MetadataKey, MetadataEntry>(exifProperties.Length);

                foreach (PaintDotNet.Imaging.ExifPropertyItem property in exifProperties)
                {
                    MetadataSection section;
                    switch (property.Path.Section)
                    {
                        case PaintDotNet.Imaging.ExifSection.Image:
                            section = MetadataSection.Image;
                            break;
                        case PaintDotNet.Imaging.ExifSection.Photo:
                            section = MetadataSection.Exif;
                            break;
                        case PaintDotNet.Imaging.ExifSection.Interop:
                            section = MetadataSection.Interop;
                            break;
                        case PaintDotNet.Imaging.ExifSection.GpsInfo:
                            section = MetadataSection.Gps;
                            break;
                        default:
                            throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                                              "Unexpected {0} type: {1}",
                                                                              nameof(PaintDotNet.Imaging.ExifSection),
                                                                              (int)property.Path.Section));
                    }

                    MetadataKey metadataKey = new MetadataKey(section, property.Path.TagID);

                    if (!items.ContainsKey(metadataKey))
                    {
                        byte[] clonedData = PaintDotNet.Collections.EnumerableExtensions.ToArrayEx(property.Value.Data);

                        items.Add(metadataKey, new MetadataEntry(metadataKey, (TagDataType)property.Value.Type, clonedData));
                    }
                }
            }
#endif

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
