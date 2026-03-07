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
using PaintDotNet.FileTypes;
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
            LibWebPVersion,
            Effort,
        }

        private static readonly IReadOnlyList<string> FileExtensions = [".webp"];

        private readonly IWebPStringResourceManager strings;
        private readonly IServiceProvider? serviceProvider;

        public WebPFileType(IFileTypeHost host)
            : base(host,
                   "WebP",
                   FileTypeOptions.Create() with
                   {
                       LoadExtensions = FileExtensions,
                       SaveExtensions = FileExtensions,
                       IsSavingConfigurable = true
                   })
        {
            if (host != null)
            {
                strings = new PdnLocalizedStringResourceManager(host.Services.GetService<PaintDotNet.FileTypes.WebP.IWebPFileTypeStrings>()!);
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

        private static (IBitmap<ColorBgra32>, DecoderMetadata) GetOrientedSurface(byte[] bytes)
        {
            (IBitmap<ColorBgra32> bitmap, DecoderMetadata metadata) = WebPFile.Load(bytes);

            ExifValueCollection? exif = metadata.Exif;

            if (exif != null)
            {
                ExifValue? orientationProperty = exif.GetAndRemoveValue(ExifPropertyKeys.Image.Orientation.Path);
                if (orientationProperty != null)
                {
                    MetadataHelpers.ApplyOrientationTransform(orientationProperty, ref bitmap);
                }
            }

            return (bitmap, metadata);
        }

        protected override PropertyBasedFileTypeLoader OnCreatePropertyBasedLoader()
        {
            return new Loader(this);
        }

        private sealed class Loader
            : PropertyBasedFileTypeLoader
        {
            public Loader(WebPFileType fileType)
                : base(fileType)
            {
            }

            protected override IFileTypeDocument OnLoad(IPropertyBasedFileTypeLoadContext context)
            {
                byte[] bytes = new byte[context.Input.Length];

                context.Input.ReadExactly(bytes, 0, (int)context.Input.Length);

                if (FormatDetection.HasWebPFileSignature(bytes))
                {
                    (IBitmap<ColorBgra32> bitmap, DecoderMetadata metadata) = GetOrientedSurface(bytes);
                    IFileTypeDocument<ColorBgra32>? doc = context.Factory.CreateDocument<ColorBgra32>(bitmap.Size);

                    byte[]? colorProfileBytes = metadata.GetColorProfileBytes();
                    if (colorProfileBytes != null)
                    {
                        using IColorContext colorContext = this.Services.GetService<IImagingFactory>()!.CreateColorContext(colorProfileBytes);
                        doc.SetColorContext(colorContext);
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
                                            doc.Resolution = new Resolution(xRes, yRes, MeasurementUnit.Centimeter);
                                            break;
                                        case TiffConstants.ResolutionUnit.Inch:
                                            doc.Resolution = new Resolution(xRes, yRes, MeasurementUnit.Inch);
                                            break;
                                    }
                                }
                            }
                        }

                        using (IFileTypeExifMetadataTransaction exifTx = doc.Metadata.Exif.CreateTransaction())
                        {
                            foreach (KeyValuePair<ExifPropertyPath, ExifValue> item in exifMetadata)
                            {
                                ExifPropertyPath path = item.Key;

                                exifTx.SetItem(path, item.Value);
                            }
                        }
                    }

                    byte[]? xmpBytes = metadata.GetXmpBytes();
                    if (xmpBytes != null)
                    {
                        XmpPacket? xmpPacket = XmpPacket.TryParse(xmpBytes);
                        using (IFileTypeXmpMetadataTransaction xmpTx = doc.Metadata.Xmp.CreateTransaction())
                        {
                            xmpTx.XmpPacket = xmpPacket;
                        }
                    }

                    using IFileTypeBitmapLayer<ColorBgra32> layer = doc.CreateBitmapLayer();
                    using IFileTypeBitmapSink<ColorBgra32> layerSink = layer.GetBitmap();
                    layerSink.WriteSource(bitmap);
                    doc.Layers.Add(layer);

                    bitmap.Dispose();

                    return doc;
                }
                else
                {
                    // The file may be a JPEG or PNG that has the wrong file extension.
                    IFileTypeInfo? fileTypeInfo = FormatDetection.TryGetFileTypeInfo(bytes, this.Services);

                    if (fileTypeInfo != null)
                    {
                        using IFileType fileType = fileTypeInfo.CreateInstance();
                        using IFileTypeLoader loader = fileType.CreateLoader();

                        using (MemoryStream stream = new(bytes))
                        {
                            return loader.Load(stream);
                        }
                    }
                    else
                    {
                        throw new FormatException(Properties.Resources.InvalidWebPImage);
                    }
                }
            }
        }

        protected override PropertyBasedFileTypeSaver OnCreatePropertyBasedSaver()
        {
            return new Saver(this);
        }

        private sealed class Saver
            : PropertyBasedFileTypeSaver
        {
            private readonly WebPFileType fileType;

            public Saver(WebPFileType fileType)
                : base(fileType)
            {
                this.fileType = fileType;
            }

            protected override PropertyCollection OnCreateDefaultSaveProperties()
            {
                List<Property> props =
                [
                    StaticListChoiceProperty.CreateForEnum(PropertyNames.Preset, WebPPreset.Photo, false),
                    new Int32Property(PropertyNames.Quality, 95, 0, 100, false),
                    new Int32Property(PropertyNames.Effort, 7, 0, 9, false),
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

            protected override ControlInfo OnCreateSaveOptionsUI(PropertyCollection props)
            {
                ControlInfo info = CreateDefaultSaveOptionsUI(props);

                PropertyControlInfo presetPCI = info.FindControlForPropertyName(PropertyNames.Preset)!;

                presetPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = this.fileType.GetString("Preset_DisplayName");
                presetPCI.SetValueDisplayName(WebPPreset.Default, this.fileType.GetString("Preset_Default_DisplayName"));
                presetPCI.SetValueDisplayName(WebPPreset.Drawing, this.fileType.GetString("Preset_Drawing_DisplayName"));
                presetPCI.SetValueDisplayName(WebPPreset.Icon, this.fileType.GetString("Preset_Icon_DisplayName"));
                presetPCI.SetValueDisplayName(WebPPreset.Photo, this.fileType.GetString("Preset_Photo_DisplayName"));
                presetPCI.SetValueDisplayName(WebPPreset.Picture, this.fileType.GetString("Preset_Picture_DisplayName"));
                presetPCI.SetValueDisplayName(WebPPreset.Text, this.fileType.GetString("Preset_Text_DisplayName"));

                info.SetPropertyControlValue(PropertyNames.Quality, ControlInfoPropertyNames.DisplayName, this.fileType.GetString("Quality_DisplayName"));
                info.SetPropertyControlValue(PropertyNames.Effort, ControlInfoPropertyNames.DisplayName, this.fileType.GetString("Effort_DisplayName"));

                PropertyControlInfo losslessPCI = info.FindControlForPropertyName(PropertyNames.Lossless)!;
                losslessPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = string.Empty;
                losslessPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = this.fileType.GetString("Lossless_Description");

                PropertyControlInfo forumLinkPCI = info.FindControlForPropertyName(PropertyNames.ForumLink)!;
                forumLinkPCI.ControlProperties[ControlInfoPropertyNames.DisplayName]!.Value = this.fileType.GetString("ForumLink_DisplayName");
                forumLinkPCI.ControlProperties[ControlInfoPropertyNames.Description]!.Value = this.fileType.GetString("ForumLink_Description");

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

            protected override void OnSave(IPropertyBasedFileTypeSaveContext context)
            {
                IPropertyBasedFileTypeSaveOptions options = context.Options;

                int quality = options.GetProperty<Int32Property>(PropertyNames.Quality)!.Value;
                int effort = options.GetProperty<Int32Property>(PropertyNames.Effort)!.Value;
                WebPPreset preset = (WebPPreset)options.GetProperty(PropertyNames.Preset)!.Value!;
                bool lossless = options.GetProperty<BooleanProperty>(PropertyNames.Lossless)!.Value;

                IImagingFactory imagingFactory = this.Services.GetService<IImagingFactory>()!;
                WebPFile.Save(context.Document, context.Output, quality, effort, preset, lossless, context.ProgressCallback, imagingFactory);
            }
        }
    }
}
