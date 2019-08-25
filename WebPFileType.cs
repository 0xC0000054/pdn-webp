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
        private enum PropertyNames
        {
            Preset,
            Quality,
            KeepMetadata
        }

        public WebPFileType()
            : base("WebP",
                  FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving | FileTypeFlags.SavesWithProgress,
                  new string[] { ".webp" })
        {
        }

        public FileType[] GetFileTypeInstances()
        {
            return new FileType[] { new WebPFileType()};
        }

        private static Document GetOrientedDocument(byte[] bytes, out ExifValueCollection exifMetadata)
        {
            exifMetadata = null;

            Document doc = null;

            // Load the image into a Bitmap so the EXIF orientation transform can be applied.

            using (Bitmap image = WebPFile.Load(bytes))
            {
                byte[] exifBytes = WebPFile.GetExifBytes(bytes);
                if (exifBytes != null)
                {
                    exifMetadata = ExifParser.Parse(exifBytes);

                    if (exifMetadata.Count > 0)
                    {
                        MetadataEntry orientationProperty = exifMetadata.GetAndRemoveValue(ExifTagID.Orientation);
                        if (orientationProperty != null)
                        {
                            RotateFlipType transform = MetadataHelpers.GetOrientationTransform(orientationProperty);
                            if (transform != RotateFlipType.RotateNoneFlipNone)
                            {
                                image.RotateFlip(transform);
                            }
                        }

                        MetadataEntry xResProperty = exifMetadata.GetAndRemoveValue(ExifTagID.XResolution);
                        MetadataEntry yResProperty = exifMetadata.GetAndRemoveValue(ExifTagID.YResolution);
                        MetadataEntry resUnitProperty = exifMetadata.GetAndRemoveValue(ExifTagID.ResolutionUnit);
                        if (xResProperty != null && yResProperty != null && resUnitProperty != null)
                        {
                            if (MetadataHelpers.TryDecodeRational(xResProperty, out double xRes) &&
                                MetadataHelpers.TryDecodeRational(yResProperty, out double yRes) &&
                                MetadataHelpers.TryDecodeShort(resUnitProperty, out ushort resUnit))
                            {
                                if (xRes > 0.0 && yRes > 0.0)
                                {
                                    double dpiX, dpiY;

                                    switch ((MeasurementUnit)resUnit)
                                    {
                                        case MeasurementUnit.Centimeter:
                                            dpiX = Document.DotsPerCmToDotsPerInch(xRes);
                                            dpiY = Document.DotsPerCmToDotsPerInch(yRes);
                                            break;
                                        case MeasurementUnit.Inch:
                                            dpiX = xRes;
                                            dpiY = yRes;
                                            break;
                                        default:
                                            // Unknown ResolutionUnit value.
                                            dpiX = 0.0;
                                            dpiY = 0.0;
                                            break;
                                    }

                                    if (dpiX > 0.0 && dpiY > 0.0)
                                    {
                                        try
                                        {
                                            image.SetResolution((float)dpiX, (float)dpiY);
                                        }
                                        catch
                                        {
                                            // Ignore any errors when setting the resolution.
                                        }
                                    }
                                }
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

            Document doc = GetOrientedDocument(bytes, out ExifValueCollection exifMetadata);

            byte[] colorProfileBytes = WebPFile.GetColorProfileBytes(bytes);
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
                foreach (MetadataEntry entry in exifMetadata)
                {
                    PropertyItem propertyItem = entry.TryCreateGdipPropertyItem();

                    if (propertyItem != null)
                    {
                        doc.Metadata.AddExifValues(new PropertyItem[] { propertyItem });
                    }
                }
            }

            byte[] xmpBytes = WebPFile.GetXmpBytes(bytes);
            if (xmpBytes != null)
            {
                doc.Metadata.SetUserValue(WebPMetadataNames.XMP, Convert.ToBase64String(xmpBytes, Base64FormattingOptions.None));
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

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {
            int quality = token.GetProperty<Int32Property>(PropertyNames.Quality).Value;
            WebPPreset preset = (WebPPreset)token.GetProperty(PropertyNames.Preset).Value;
            bool keepMetadata = token.GetProperty<BooleanProperty>(PropertyNames.KeepMetadata).Value;

            WebPFile.Save(input, output, quality, preset, keepMetadata, scratchSurface, progressCallback);
        }
    }
}
