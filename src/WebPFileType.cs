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

using System;
using System.Collections.Generic;
using System.IO;
using PaintDotNet;
using PaintDotNet.Imaging;
using PaintDotNet.IndirectUI;
using PaintDotNet.PropertySystem;
using WebPFileType.Exif;
using WebPFileType.Interop;

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
            GitHubLink,
            Lossless,
            PluginVersion,
            LibWebPVersion
        }

        private static readonly IReadOnlyList<string> FileExtensions = [".webp"];

        private readonly IWebPStringResourceManager strings;
        private readonly IServiceProvider? serviceProvider;

        public WebPFileType(IFileTypeHost host)
            : base("WebP",
                  new FileTypeOptions
                  {
                      LoadExtensions = FileExtensions,
                      SaveExtensions = FileExtensions
                  })
        {
            if (host != null)
            {
                strings = new PdnLocalizedStringResourceManager(host.Services.GetService<PaintDotNet.WebP.IWebPFileTypeStrings>()!);
                serviceProvider = host.Services;
            }
            else
            {
                strings = new BuiltinStringResourceManager();
                serviceProvider = null;
            }
        }

        private string GetString(string name)
        {
            return strings.GetString(name);
        }

        private static (Surface, DecoderMetadata) GetOrientedSurface(byte[] bytes)
        {
            (Surface surface, DecoderMetadata metadata) = WebPFile.Load(bytes);

            ExifValueCollection? exif = metadata.Exif;

            if (exif != null)
            {
                ExifValue? orientationProperty = exif.GetAndRemoveValue(ExifPropertyKeys.Image.Orientation.Path);
                if (orientationProperty != null)
                {
                    MetadataHelpers.ApplyOrientationTransform(orientationProperty, ref surface);
                }
            }

            return (surface, metadata);
        }

        protected override Document OnLoad(Stream input)
        {
            byte[] bytes = new byte[input.Length];

            input.ReadExactly(bytes, 0, (int)input.Length);
            Document? doc = null;

            if (FormatDetection.HasWebPFileSignature(bytes))
            {
                (Surface surface, DecoderMetadata metadata) = GetOrientedSurface(bytes);
                bool disposeSurface = true;

                try
                {
                    doc = new Document(surface.Width, surface.Height);

                    byte[]? colorProfileBytes = metadata.GetColorProfileBytes();
                    if (colorProfileBytes != null)
                    {
                        doc.Metadata.AddExifPropertyItem(ExifSection.Image,
                                                         ExifPropertyKeys.Image.InterColorProfile.Path.TagID,
                                                         new ExifValue(ExifValueType.Undefined,
                                                                       colorProfileBytes));
                    }

                    ExifValueCollection? exifMetadata = metadata.Exif;
                    if (exifMetadata != null && exifMetadata.Count > 0)
                    {
                        ExifValue? xResProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.XResolution.Path);
                        ExifValue? yResProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.YResolution.Path);
                        ExifValue? resUnitProperty = exifMetadata.GetAndRemoveValue(ExifPropertyKeys.Image.ResolutionUnit.Path);

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

                    byte[]? xmpBytes = metadata.GetXmpBytes();
                    if (xmpBytes != null)
                    {
                        XmpPacket? xmpPacket = XmpPacket.TryParse(xmpBytes);
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
                IFileTypeInfo? fileTypeInfo = FormatDetection.TryGetFileTypeInfo(bytes, serviceProvider);

                if (fileTypeInfo != null)
                {
                    FileType fileType = fileTypeInfo.GetInstance();

                    using (MemoryStream stream = new(bytes))
                    {
                        doc = fileType.Load(stream);
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
            List<Property> props =
            [
                StaticListChoiceProperty.CreateForEnum(PropertyNames.Preset, WebPPreset.Photo, false),
                new Int32Property(PropertyNames.Quality, 95, 0, 100, false),
                new BooleanProperty(PropertyNames.Lossless, false),
                new UriProperty(PropertyNames.ForumLink, new Uri("https://forums.getpaint.net/topic/21773-webp-filetype/")),
                new UriProperty(PropertyNames.GitHubLink, new Uri("https://github.com/0xC0000054/pdn-webp")),
                new StringProperty(PropertyNames.PluginVersion),
                new StringProperty(PropertyNames.LibWebPVersion),
            ];

            List<PropertyCollectionRule> rules =
            [
                new ReadOnlyBoundToBooleanRule(PropertyNames.Quality, PropertyNames.Lossless, false)
            ];

            return new PropertyCollection(props, rules);
        }

        public override ControlInfo OnCreateSaveConfigUI(PropertyCollection props)
        {
            ControlInfo info = CreateDefaultSaveConfigUI(props);

            PropertyControlInfo presetPCI = info.FindControlForPropertyName(PropertyNames.Preset)!;

            presetPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = GetString("Preset_DisplayName");
            presetPCI.SetValueDisplayName(WebPPreset.Default, GetString("Preset_Default_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Drawing, GetString("Preset_Drawing_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Icon, GetString("Preset_Icon_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Photo, GetString("Preset_Photo_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Picture, GetString("Preset_Picture_DisplayName"));
            presetPCI.SetValueDisplayName(WebPPreset.Text, GetString("Preset_Text_DisplayName"));

            info.SetPropertyControlValue(PropertyNames.Quality, ControlInfoPropertyNames.DisplayName, GetString("Quality_DisplayName"));

            PropertyControlInfo losslessPCI = info.FindControlForPropertyName(PropertyNames.Lossless)!;
            losslessPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = string.Empty;
            losslessPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = GetString("Lossless_Description");

            PropertyControlInfo forumLinkPCI = info.FindControlForPropertyName(PropertyNames.ForumLink)!;
            forumLinkPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = GetString("ForumLink_DisplayName");
            forumLinkPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = GetString("ForumLink_Description");

            PropertyControlInfo githubLinkPCI = info.FindControlForPropertyName(PropertyNames.GitHubLink)!;
            githubLinkPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = string.Empty;
            githubLinkPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = "GitHub"; // GitHub is a brand name that should not be localized.

            PropertyControlInfo pluginVersionPCI = info.FindControlForPropertyName(PropertyNames.PluginVersion)!;
            pluginVersionPCI.ControlType.Value = PropertyControlType.Label;
            pluginVersionPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = string.Empty;
            pluginVersionPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = "WebPFileType v" + VersionInfo.PluginVersion;

            PropertyControlInfo libwebpVersionPCI = info.FindControlForPropertyName(PropertyNames.LibWebPVersion)!;
            libwebpVersionPCI.ControlType.Value = PropertyControlType.Label;
            libwebpVersionPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = string.Empty;
            libwebpVersionPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = "libwebp v" + VersionInfo.LibWebPVersion;

            return info;
        }

        protected override void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback)
        {
            int quality = token.GetProperty<Int32Property>(PropertyNames.Quality)!.Value;
            WebPPreset preset = (WebPPreset)token.GetProperty(PropertyNames.Preset)!.Value!;
            bool lossless = token.GetProperty<BooleanProperty>(PropertyNames.Lossless)!.Value;

            WebPFile.Save(input, output, quality, preset, lossless, scratchSurface, progressCallback);
        }
    }
}
