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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using PaintDotNet;
using PaintDotNet.IO;
using WebPFileType.Properties;

namespace WebPFileType
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    public sealed class WebPFileType : FileType, IFileTypeFactory
    {
        private const string WebPColorProfile = "WebPICC";
        private const string WebPEXIF = "WebPEXIF";
        private const string WebPXMP = "WebPXMP";

        public WebPFileType() : base("WebP", FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving | FileTypeFlags.SavesWithProgress, new string[] { ".webp" })
        {
        }

        public FileType[] GetFileTypeInstances()
        {
            return new FileType[] { new WebPFileType()};
        }

        private static string GetMetaDataBase64(byte[] data, WebPFile.MetaDataType type, uint metaDataSize)
        {
            byte[] bytes = new byte[metaDataSize];
            WebPFile.ExtractMetadata(data, type, bytes, metaDataSize);

            return Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        }

        protected override Document OnLoad(Stream input)
        {
            byte[] bytes = new byte[input.Length];

            input.ProperRead(bytes, 0, (int)input.Length);

            int width = 0;
            int height = 0;
            if (!WebPFile.WebPGetDimensions(bytes, out width, out height))
            {
                throw new WebPException(Resources.InvalidWebPImage);
            }

            Document doc = new Document(width, height);
            BitmapLayer layer = Layer.CreateBackgroundLayer(width, height);

            unsafe
            {
                WebPFile.VP8StatusCode status = WebPFile.WebPLoad(bytes, layer.Surface);
                if (status != WebPFile.VP8StatusCode.Ok)
                {
                    switch (status)
                    {
                        case WebPFile.VP8StatusCode.OutOfMemory:
                            throw new OutOfMemoryException();
                        case WebPFile.VP8StatusCode.InvalidParam:
                            throw new WebPException(Resources.InvalidParameter);
                        case WebPFile.VP8StatusCode.BitStreamError:
                        case WebPFile.VP8StatusCode.UnsupportedFeature:
                        case WebPFile.VP8StatusCode.NotEnoughData:
                            throw new WebPException(Resources.InvalidWebPImage);
                        default:
                            break;
                    }
                }

                uint colorProfileSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.ColorProfile);
                if (colorProfileSize > 0U)
                {
                    string icc = GetMetaDataBase64(bytes, WebPFile.MetaDataType.ColorProfile, colorProfileSize);
                    doc.Metadata.SetUserValue(WebPColorProfile, icc);
                }

                uint exifSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.EXIF);
                if (exifSize > 0)
                {
                    string exif = GetMetaDataBase64(bytes, WebPFile.MetaDataType.EXIF, exifSize);
                    doc.Metadata.SetUserValue(WebPEXIF, exif);
                }

                uint xmpSize = WebPFile.GetMetaDataSize(bytes, WebPFile.MetaDataType.XMP);
                if (xmpSize > 0U)
                {
                    string xmp = GetMetaDataBase64(bytes, WebPFile.MetaDataType.XMP, xmpSize);
                    doc.Metadata.SetUserValue(WebPXMP, xmp);
                }
            }

            doc.Layers.Add(layer);

            return doc;
        }

        public override SaveConfigWidget CreateSaveConfigWidget()
        {
            return new WebPSaveConfigWidget();
        }

        protected override SaveConfigToken OnCreateDefaultSaveConfigToken()
        {
            return new WebPSaveConfigToken(WebPPreset.Photo, 95, 4, 80, 30, 3, WebPFilterType.Strong, 0, true);
        }

        public override bool IsReflexive(SaveConfigToken token)
        {
            if (((WebPSaveConfigToken)token).Quality == 100)
            {
                return true;
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1075", Justification = "Ignore any errors thrown by SetResolution.")]
        private static void LoadProperties(Image dstImage, MeasurementUnit dpuUnit, double dpuX, double dpuY, IEnumerable<PropertyItem> propertyItems)
        {
            Bitmap asBitmap = dstImage as Bitmap;

            if (asBitmap != null)
            {
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
                    asBitmap.SetResolution(dpiX, dpiY);
                }
                catch (Exception)
                {
                    // Ignore error
                }
            }

            foreach (PropertyItem pi in propertyItems)
            {
                try
                {
                    dstImage.SetPropertyItem(pi);
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

                        items.Add(pi);
                    }
                    catch
                    {
                        // Ignore any items that cannot be deserialized.
                    }
                }
            }

            return items;
        }

        private static WebPFile.MetaDataParams GetMetaData(Document doc, Surface scratchSurface)
        {
            byte[] iccProfileBytes = null;
            byte[] exifBytes = null;
            byte[] xmpBytes = null;

            string colorProfile = doc.Metadata.GetUserValue(WebPColorProfile);
            if (!string.IsNullOrEmpty(colorProfile))
            {
                iccProfileBytes = Convert.FromBase64String(colorProfile);
            }

            string exif = doc.Metadata.GetUserValue(WebPEXIF);
            if (!string.IsNullOrEmpty(exif))
            {
                exifBytes = Convert.FromBase64String(exif);
            }

            string xmp = doc.Metadata.GetUserValue(WebPXMP);
            if (!string.IsNullOrEmpty(xmp))
            {
                xmpBytes = Convert.FromBase64String(xmp);
            }

            if (iccProfileBytes == null || exifBytes == null)
            {
                List<PropertyItem> propertyItems = GetMetadataFromDocument(doc);

                if (propertyItems.Count > 0)
                {
                    const ushort ICCProfileId = unchecked((ushort)ExifTagID.IccProfileData);

                    if (iccProfileBytes == null)
                    {
                        PropertyItem item = propertyItems.FirstOrDefault(p => p.Id == ICCProfileId);

                        if (item != null)
                        {
                            iccProfileBytes = item.Value.CloneT();
                        }
                    }

                    if (exifBytes == null)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (Bitmap bmp = scratchSurface.CreateAliasedBitmap())
                            {
                                LoadProperties(bmp, doc.DpuUnit, doc.DpuX, doc.DpuY, propertyItems.Where(p => p.Id != ICCProfileId));
                                bmp.Save(stream, ImageFormat.Jpeg);
                            }

                            exifBytes = JPEGReader.ExtractEXIF(stream.GetBuffer());
                        }
                    }
                }
            }

            if (iccProfileBytes != null || exifBytes != null || xmpBytes != null)
            {
                return new WebPFile.MetaDataParams(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        protected override void OnSave(Document input, Stream output, SaveConfigToken token, Surface scratchSurface, ProgressEventHandler callback)
        {
            WebPSaveConfigToken configToken = (WebPSaveConfigToken)token;

            WebPFile.WebPReportProgress encProgress = new WebPFile.WebPReportProgress(delegate(int percent)
            {
                callback(this, new ProgressEventArgs(percent));
            });

            WebPFile.EncodeParams encParams = new WebPFile.EncodeParams
            {
                quality = (float)configToken.Quality,
                preset = configToken.Preset,
                method = configToken.Method,
                noiseShaping = configToken.NoiseShaping,
                filterType = configToken.FilterType,
                filterStrength = configToken.FilterStrength,
                sharpness = configToken.Sharpness,
                fileSize = configToken.FileSize
            };

            using (RenderArgs ra = new RenderArgs(scratchSurface))
            {
                input.Render(ra, true);
            }

            using (PinnedByteArrayAllocator allocator = new PinnedByteArrayAllocator())
            {
                WebPFile.MetaDataParams metaData = null;
                if (configToken.EncodeMetaData)
                {
                    metaData = GetMetaData(input, scratchSurface);
                }

                IntPtr pinnedArrayPtr = WebPFile.WebPSave(allocator, scratchSurface, encParams, metaData, encProgress);

                if (pinnedArrayPtr != IntPtr.Zero)
                {
                    byte[] data = allocator.GetManagedArray(pinnedArrayPtr);

                    output.Write(data, 0, data.Length);
                }
            }
        }
    }
}
