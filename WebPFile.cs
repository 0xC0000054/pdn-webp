////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2019 Nicholas Hayes
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
using System.Runtime.InteropServices;
using WebPFileType.Exif;
using WebPFileType.Properties;

namespace WebPFileType
{
    static class WebPFile
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

            int width;
            int height;
            if (!WebPNative.WebPGetDimensions(webpBytes, out width, out height))
            {
                throw new WebPException(Resources.InvalidWebPImage);
            }

            Bitmap image = null;
            Bitmap temp = null;

            try
            {
                temp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                BitmapData bitmapData = temp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

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
        /// <param name="keepMetadata"><c>true</c> if metadata should be preserved; otherwise <c>false</c>.</param>
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
            bool keepMetadata,
            Surface scratchSurface,
            ProgressEventHandler progressCallback)
        {
            WebPNative.EncodeParams encParams = new WebPNative.EncodeParams
            {
                quality = quality,
                preset = preset
            };

            using (RenderArgs ra = new RenderArgs(scratchSurface))
            {
                input.Render(ra, true);
            }

            WebPNative.MetadataParams metadata = null;
            if (keepMetadata)
            {
                metadata = CreateWebPMetadata(input, scratchSurface);
            }

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

            WebPNative.WebPSave(WriteImageCallback, scratchSurface, encParams, metadata, encProgress);

            void WriteImageCallback(IntPtr image, UIntPtr imageSize)
            {
                // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
                const int MaxBufferSize = 81920;

                long size = checked((long)imageSize.ToUInt64());

                int bufferSize = (int)Math.Min(size, MaxBufferSize);

                byte[] streamBuffer = new byte[bufferSize];

                output.SetLength(size);

                long offset = 0;
                long remaining = size;

                while (remaining > 0)
                {
                    int copySize = (int)Math.Min(MaxBufferSize, remaining);

                    Marshal.Copy(new IntPtr(image.ToInt64() + offset), streamBuffer, 0, copySize);

                    output.Write(streamBuffer, 0, copySize);

                    offset += copySize;
                    remaining -= copySize;
                }
            }
        }

        private static WebPNative.MetadataParams CreateWebPMetadata(Document doc, Surface scratchSurface)
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
                List<PropertyItem> propertyItems = GetMetadataFromDocument(doc);

                if (propertyItems.Count > 0)
                {
                    if (iccProfileBytes == null)
                    {
                        const int ICCProfileId = unchecked((ushort)ExifTagID.IccProfileData);

                        PropertyItem item = propertyItems.Find(p => p.Id == ICCProfileId);

                        if (item != null)
                        {
                            iccProfileBytes = item.Value.CloneT();
                            propertyItems.RemoveAll(p => p.Id == ICCProfileId);
                        }
                    }

                    if (exifBytes == null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (Bitmap bmp = scratchSurface.CreateAliasedBitmap())
                            {
                                LoadProperties(bmp, doc.DpuUnit, doc.DpuX, doc.DpuY, propertyItems);
                                bmp.Save(stream, ImageFormat.Jpeg);
                            }

                            exifBytes = JPEGReader.ExtractEXIF(stream);
                        }
                    }
                }
            }

            if (iccProfileBytes != null || exifBytes != null || xmpBytes != null)
            {
                return new WebPNative.MetadataParams(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1075", Justification = "Ignore any errors thrown by SetResolution.")]
        private static void LoadProperties(Bitmap bitmap, MeasurementUnit dpuUnit, double dpuX, double dpuY, IEnumerable<PropertyItem> propertyItems)
        {
            // The following code is from Paint.NET 3.36.

            // Sometimes GDI+ does not honor the resolution tags that we
            // put in manually via the EXIF properties.
            float dpiX;
            float dpiY;

            switch (dpuUnit)
            {
                case MeasurementUnit.Centimeter:
                    dpiX = (float)Document.DotsPerCmToDotsPerInch(dpuX);
                    dpiY = (float)Document.DotsPerCmToDotsPerInch(dpuY);
                    break;

                case MeasurementUnit.Inch:
                    dpiX = (float)dpuX;
                    dpiY = (float)dpuY;
                    break;

                default:
                case MeasurementUnit.Pixel:
                    dpiX = 1.0f;
                    dpiY = 1.0f;
                    break;
            }

            try
            {
                bitmap.SetResolution(dpiX, dpiY);
            }
            catch (Exception)
            {
                // Ignore error
            }

            foreach (PropertyItem pi in propertyItems)
            {
                try
                {
                    bitmap.SetPropertyItem(pi);
                }
                catch (ArgumentException)
                {
                    // Ignore error: the image does not support property items
                }
            }
        }

        private static List<PropertyItem> GetMetadataFromDocument(Document doc)
        {
            List<PropertyItem> items = new List<PropertyItem>();

            Metadata metadata = doc.Metadata;

            string[] exifKeys = metadata.GetKeys(Metadata.ExifSectionName);

            if (exifKeys.Length > 0)
            {
                items.Capacity = exifKeys.Length;

                foreach (string key in exifKeys)
                {
                    string blob = metadata.GetValue(Metadata.ExifSectionName, key);
                    try
                    {
                        PropertyItem pi = PaintDotNet.SystemLayer.PdnGraphics.DeserializePropertyItem(blob);

                        // GDI+ does not support the Interoperability IFD tags.
                        if (!IsInteroperabilityIFDTag(pi))
                        {
                            items.Add(pi);
                        }
                    }
                    catch
                    {
                        // Ignore any items that cannot be deserialized.
                    }
                }
            }

            return items;

            bool IsInteroperabilityIFDTag(PropertyItem propertyItem)
            {
                if (propertyItem.Id == 1)
                {
                    // The tag number 1 is used by both the GPS IFD (GPSLatitudeRef) and the Interoperability IFD (InteroperabilityIndex).
                    // The EXIF specification states that InteroperabilityIndex should be a four character ASCII field.

                    return propertyItem.Type == (short)ExifTagType.Ascii && propertyItem.Len == 4;
                }
                else if (propertyItem.Id == 2)
                {
                    // The tag number 2 is used by both the GPS IFD (GPSLatitude) and the Interoperability IFD (InteroperabilityVersion).
                    // The DCF specification states that InteroperabilityVersion should be a four byte field.
                    switch ((ExifTagType)propertyItem.Type)
                    {
                        case ExifTagType.Byte:
                        case ExifTagType.Undefined:
                            return propertyItem.Len == 4;
                        default:
                            return false;
                    }
                }
                else
                {
                    switch (propertyItem.Id)
                    {
                        case 4096: // Interoperability IFD - RelatedImageFileFormat
                        case 4097: // Interoperability IFD - RelatedImageWidth
                        case 4098: // Interoperability IFD - RelatedImageHeight
                            return true;
                        default:
                            return false;
                    }
                }
            }
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
