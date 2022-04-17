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

using System;
using System.Collections.Generic;
using System.IO;
using PaintDotNet;
using PaintDotNet.Imaging;
using PaintDotNet.IndirectUI;
using PaintDotNet.IO;
using PaintDotNet.PropertySystem;
using WebPFileType.Exif;

namespace WebPFileType
{
    [PluginSupportInfo(typeof(PluginSupportInfo))]
    public sealed class WebPFileType :
        PropertyBasedFileType
    {
        private enum PropertyNames
        {
            Preset,
            Quality,
            KeepMetadata, // Obsolete, but retained to keep the value from being reused for a different property.
            ForumLink,
            GitHubLink
        }

        private readonly IWebPStringResourceManager strings;

        public WebPFileType(IFileTypeHost host)
            : base("WebP",
                  new FileTypeOptions
                  {
                      LoadExtensions = new string[] { ".webp" },
                      SaveExtensions = new string[] { ".webp" }
                  })
        {
            if (host != null)
            {
                strings = new PdnLocalizedStringResourceManager(host.Services.GetService<PaintDotNet.WebP.IWebPFileTypeStrings>());
            }
            else
            {
                strings = new BuiltinStringResourceManager();
            }
        }

        private string GetString(string name)
        {
            return strings.GetString(name);
        }

        private static Surface GetOrientedSurface(byte[] bytes, out ExifValueCollection exifMetadata)
        {
            exifMetadata = null;

            Surface surface = WebPFile.Load(bytes);

            byte[] exifBytes = WebPFile.GetExifBytes(bytes);
            if (exifBytes != null)
            {
                exifMetadata = ExifParser.Parse(exifBytes);

                if (exifMetadata != null)
                {
                    ExifValue orientationProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.Orientation.Path);
                    if (orientationProperty != null)
                    {
                        MetadataHelpers.ApplyOrientationTransform(orientationProperty, ref surface);
                    }
                }
            }

            return surface;
        }

        protected override Document OnLoad(Stream input)
        {
            byte[] bytes = new byte[input.Length];

            input.ProperRead(bytes, 0, (int)input.Length);
            Document doc = null;

            if (FormatDetection.HasWebPFileSignature(bytes))
            {
                Surface surface = GetOrientedSurface(bytes, out ExifValueCollection exifMetadata);
                bool disposeSurface = true;

                try
                {
                    doc = new Document(surface.Width, surface.Height);

                    byte[] colorProfileBytes = WebPFile.GetColorProfileBytes(bytes);
                    if (colorProfileBytes != null)
                    {
                        doc.Metadata.AddExifPropertyItem(ExifSection.Image,
                                                         unchecked((ushort)ExifTagID.IccProfileData),
                                                         new ExifValue(ExifValueType.Undefined,
                                                                       colorProfileBytes));
                    }

                    if (exifMetadata != null && exifMetadata.Count > 0)
                    {
                        ExifValue xResProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.XResolution.Path);
                        ExifValue yResProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.YResolution.Path);
                        ExifValue resUnitProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.ResolutionUnit.Path);

                        if (xResProperty != null && yResProperty != null && resUnitProperty != null)
                        {
                            if (MetadataHelpers.TryDecodeRational(xResProperty, out double xRes) &&
                                MetadataHelpers.TryDecodeRational(yResProperty, out double yRes) &&
                                MetadataHelpers.TryDecodeShort(resUnitProperty, out ushort resUnit))
                            {
                                if (xRes > 0.0 && yRes > 0.0)
                                {
                                    switch (resUnit)
                                    {
                                        case TiffConstants.ResolutionUnit.Centimeter:
                                            doc.DpuUnit = MeasurementUnit.Centimeter;
                                            doc.DpuX = xRes;
                                            doc.DpuY = yRes;
                                            break;
                                        case TiffConstants.ResolutionUnit.Inch:
                                            doc.DpuUnit = MeasurementUnit.Inch;
                                            doc.DpuX = xRes;
                                            doc.DpuY = yRes;
                                            break;
                                    }
                                }
                            }
                        }

                        foreach (KeyValuePair<ExifPropertyPath, ExifValue> item in exifMetadata)
                        {
                            ExifPropertyPath path = item.Key;

                            doc.Metadata.AddExifPropertyItem(path.Section, path.TagID, item.Value);
                        }
                    }

                    byte[] xmpBytes = WebPFile.GetXmpBytes(bytes);
                    if (xmpBytes != null)
                    {
                        XmpPacket xmpPacket = XmpPacket.TryParse(xmpBytes);
                        if (xmpPacket != null)
                        {
                            doc.Metadata.SetXmpPacket(xmpPacket);
                        }
                    }

                    doc.Layers.Add(Layer.CreateBackgroundLayer(surface, takeOwnership: true));
                    disposeSurface = false;
                }
                finally
                {
                    if (disposeSurface)
                    {
                        surface?.Dispose();
                    }
                }
            }
            else
            {
                // The file may be a JPEG or PNG that has the wrong file extension.
                if (FormatDetection.HasCommonImageFormatSignature(bytes))
                {
                    using (MemoryStream stream = new(bytes))
                    using (System.Drawing.Image image = System.Drawing.Image.FromStream(stream))
                    {
                        doc = Document.FromGdipImage(image);
                    }
                }
                else
                {
                    throw new FormatException(Properties.Resources.InvalidWebPImage);
                }
            }

            return doc;
        }

        public override PropertyCollection OnCreateSavePropertyCollection()
        {
            List<Property> props = new()
            {
                StaticListChoiceProperty.CreateForEnum(PropertyNames.Preset, WebPPreset.Photo, false),
                new Int32Property(PropertyNames.Quality, 95, 0, 100, false),
                new UriProperty(PropertyNames.ForumLink, new Uri("https://forums.getpaint.net/topic/21773-webp-filetype/")),
                new UriProperty(PropertyNames.GitHubLink, new Uri("https://github.com/0xC0000054/pdn-webp"))
            };

            return new PropertyCollection(props);
        }

        public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
        {
            ControlInfo info = CreateDefaultSaveConfigUI(props);

            PropertyControlInfo presetPCI = info.FindControlForPropertyName(PropertyNames.Preset);

            presetPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = GetString("Preset_DisplayName");
            presetPCI.SetValueDisplayName(WebPPreset.Default, GetString("Preset_Default_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Drawing, GetString("Preset_Drawing_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Icon, GetString("Preset_Icon_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Photo, GetString("Preset_Photo_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Picture, GetString("Preset_Picture_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Text, GetString("Preset_Text_DisplayName"));

            info.SetPropertyControlValue(PropertyNames.Quality, ControlInfoPropertyNames.DisplayName, GetString("Quality_DisplayName"));

            PropertyControlInfo forumLinkPCI = info.FindControlForPropertyName(PropertyNames.ForumLink);
            forumLinkPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = GetString("ForumLink_DisplayName");
            forumLinkPCI.ControlProperties[ControlInfoPropertyNames.Description].Value = GetString("ForumLink_Description");

            PropertyControlInfo githubLinkPCI = info.FindControlForPropertyName(PropertyNames.GitHubLink);
            githubLinkPCI.ControlProperties[ControlInfoPropertyNames.DisplayName].Value = string.Empty;
            githubLinkPCI.ControlProperties[ControlInfoPropertyNames.Description].Value = "GitHub"; // GitHub is a brand name that should not be localized.

            return info;
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {
            int quality = token.GetProperty<Int32Property>(PropertyNames.Quality).Value;
            WebPPreset preset = (WebPPreset)token.GetProperty(PropertyNames.Preset).Value;

            WebPFile.Save(input, output, quality, preset, scratchSurface, progressCallback);
        }
    }
}
