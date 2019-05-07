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
using PaintDotNet.IndirectUI;
using PaintDotNet.IO;
using PaintDotNet.PropertySystem;
using WebPFileType.Exif;
using WebPFileType.Properties;

namespace WebPFileType
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    public sealed class WebPFileType : PropertyBasedFileType, IFileTypeFactory
    {
        private const string WebPColorProfile = "WebPICC";
        private const string WebPEXIF = "WebPEXIF";
        private const string WebPXMP = "WebPXMP";

        private enum PropertyNames
        {
            Preset,
            Quality,
            KeepMetadata
        }

        public WebPFileType() : base("WebP", FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving | FileTypeFlags.SavesWithProgress, new string[] { ".webp" })
        {
        }

        public FileType[] GetFileTypeInstances()
        {
            return new FileType[] { new WebPFileType()};
        }

        private static byte[] GetMetaDataBytes(byte[] data, WebPFile.MetadataType type)
        {
            byte[] bytes = null;

            uint size = WebPFile.GetMetadataSize(data, type);
            if (size > 0)
            {
                bytes = new byte[size];
                WebPFile.ExtractMetadata(data, type, bytes, size);
            }

            return bytes;
        }

        private static PropertyItem GetAndRemoveExifValue(ref List<PropertyItem> exifMetadata, ExifTagID tag)
        {
            int tagID = unchecked((ushort)tag);

            PropertyItem value = exifMetadata.Find(p => p.Id == tagID);

            if (value != null)
            {
                exifMetadata.RemoveAll(p => p.Id == tagID);
            }

            return value;
        }

        private static Document GetOrientedDocument(byte[] bytes, out List<PropertyItem> exifMetadata)
        {
            exifMetadata = null;

            int width;
            int height;
            if (!WebPFile.WebPGetDimensions(bytes, out width, out height))
            {
                throw new WebPException(Resources.InvalidWebPImage);
            }

            Document doc = null;

            // Load the image into a Bitmap so the EXIF orientation transform can be applied.

            using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                try
                {
                    WebPFile.VP8StatusCode status = WebPFile.WebPLoad(bytes, bitmapData);
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
                }
                finally
                {
                    image.UnlockBits(bitmapData);
                }

                byte[] exifBytes = GetMetaDataBytes(bytes, WebPFile.MetadataType.EXIF);
                if (exifBytes != null)
                {
                    exifMetadata = ExifParser.Parse(exifBytes);

                    if (exifMetadata.Count > 0)
                    {
                        PropertyItem item = GetAndRemoveExifValue(ref exifMetadata, ExifTagID.Orientation);
                        if (item != null)
                        {
                            RotateFlipType transform = PropertyItemHelpers.GetOrientationTransform(item);
                            if (transform != RotateFlipType.RotateNoneFlipNone)
                            {
                                image.RotateFlip(transform);
                            }
                        }
                    }
                }

                doc = Document.FromGdipImage(image);
            }

            return doc;
        }

        protected override Document OnLoad(Stream input)
        {
            byte[] bytes = new byte[input.Length];

            input.ProperRead(bytes, 0, (int)input.Length);

            Document doc = GetOrientedDocument(bytes, out List<PropertyItem> exifMetadata);

            byte[] colorProfileBytes = GetMetaDataBytes(bytes, WebPFile.MetadataType.ColorProfile);
            if (colorProfileBytes != null)
            {
                PropertyItem colorProfileItem = PaintDotNet.SystemLayer.PdnGraphics.CreatePropertyItem();
                colorProfileItem.Id = unchecked((ushort)ExifTagID.IccProfileData);
                colorProfileItem.Type = (short)ExifTagType.Undefined;
                colorProfileItem.Len = colorProfileBytes.Length;
                colorProfileItem.Value = colorProfileBytes.CloneT();

                doc.Metadata.AddExifValues(new PropertyItem[] { colorProfileItem });
            }

            if (exifMetadata != null)
            {
                // Set the resolution manually as Paint.NET will not use the correct values
                // when loading from the bitmap.
                PropertyItem xResProperty = GetAndRemoveExifValue(ref exifMetadata, ExifTagID.XResolution);
                PropertyItem yResProperty = GetAndRemoveExifValue(ref exifMetadata, ExifTagID.YResolution);
                PropertyItem resUnitProperty = GetAndRemoveExifValue(ref exifMetadata, ExifTagID.ResolutionUnit);
                if (xResProperty != null && yResProperty != null && resUnitProperty != null)
                {
                    double xRes, yRes;
                    ushort resUnit;

                    if (PropertyItemHelpers.TryDecodeRational(xResProperty, out xRes) &&
                        PropertyItemHelpers.TryDecodeRational(yResProperty, out yRes) &&
                        PropertyItemHelpers.TryDecodeShort(resUnitProperty, out resUnit))
                    {
                        if (xRes > 0.0 && yRes > 0.0)
                        {
                            MeasurementUnit measurementUnit = (MeasurementUnit)resUnit;
                            if (measurementUnit == MeasurementUnit.Inch || measurementUnit == MeasurementUnit.Centimeter)
                            {
                                doc.DpuX = xRes;
                                doc.DpuY = yRes;
                                doc.DpuUnit = measurementUnit;
                            }
                        }
                    }
                }

                foreach (PropertyItem item in exifMetadata)
                {
                    doc.Metadata.AddExifValues(new PropertyItem[] { item });
                }
            }

            byte[] xmpBytes = GetMetaDataBytes(bytes, WebPFile.MetadataType.XMP);
            if (xmpBytes != null)
            {
                doc.Metadata.SetUserValue(WebPXMP, Convert.ToBase64String(xmpBytes, Base64FormattingOptions.None));
            }

            return doc;
        }

        public override PropertyCollection OnCreateSavePropertyCollection()
        {
            List<Property> props = new List<Property>
            {
                StaticListChoiceProperty.CreateForEnum(PropertyNames.Preset, WebPPreset.Photo, false),
                new Int32Property(PropertyNames.Quality, 95, 0, 100, false),
                new BooleanProperty(PropertyNames.KeepMetadata, true, false)
            };

            return new PropertyCollection(props);
        }

        public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
        {
            ControlInfo info = CreateDefaultSaveConfigUI(props);

            PropertyControlInfo presetPCI = info.FindControlForPropertyName(PropertyNames.Preset);

            presetPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = Resources.Preset_Text;
            presetPCI.SetValueDisplayName(WebPPreset.Default, Resources.Preset_Default_Name);
            presetPCI.SetValueDisplayName(WebPPreset.Drawing, Resources.Preset_Drawing_Name);
            presetPCI.SetValueDisplayName(WebPPreset.Icon, Resources.Preset_Icon_Name);
            presetPCI.SetValueDisplayName(WebPPreset.Photo, Resources.Preset_Photo_Name);
            presetPCI.SetValueDisplayName(WebPPreset.Picture, Resources.Preset_Picture_Name);
            presetPCI.SetValueDisplayName(WebPPreset.Text, Resources.Preset_Text_Name);

            info.SetPropertyControlValue(PropertyNames.Quality, ControlInfoPropertyNames.DisplayName, Resources.Quality_Text);

            info.SetPropertyControlValue(PropertyNames.KeepMetadata, ControlInfoPropertyNames.DisplayName, string.Empty);
            info.SetPropertyControlValue(PropertyNames.KeepMetadata, ControlInfoPropertyNames.Description, Resources.KeepMetadata_Text);

            return info;
        }

        protected override bool IsReflexive(PropertyBasedSaveConfigToken token)
        {
            int quality = token.GetProperty<Int32Property>(PropertyNames.Quality).Value;

            return quality == 100;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1075", Justification = "Ignore any errors thrown by SetResolution.")]
        private static void LoadProperties(Bitmap bitmap, MeasurementUnit dpuUnit, double dpuX, double dpuY, IEnumerable<PropertyItem> propertyItems)
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

        private static WebPFile.MetadataParams GetMetaData(Document doc, Surface scratchSurface)
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
                return new WebPFile.MetadataParams(iccProfileBytes, exifBytes, xmpBytes);
            }

            return null;
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {

            WebPFile.WebPReportProgress encProgress = new WebPFile.WebPReportProgress(delegate(int percent)
            {
                progressCallback(this, new ProgressEventArgs(percent));
            });

            int quality = token.GetProperty<Int32Property>(PropertyNames.Quality).Value;
            WebPPreset preset = (WebPPreset)token.GetProperty(PropertyNames.Preset).Value;
            bool keepMetadata = token.GetProperty<BooleanProperty>(PropertyNames.KeepMetadata).Value;

            WebPFile.EncodeParams encParams = new WebPFile.EncodeParams
            {
                quality = quality,
                preset = preset
            };

            using (RenderArgs ra = new RenderArgs(scratchSurface))
            {
                input.Render(ra, true);
            }

            using (PinnedByteArrayAllocator allocator = new PinnedByteArrayAllocator())
            {
                WebPFile.MetadataParams metaData = null;
                if (keepMetadata)
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
