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
using System.Diagnostics;

namespace WebPFileType.Exif
{
    [DebuggerDisplay("{DebuggerDisplay, nq}")]
    internal sealed class MetadataEntry : IEquatable<MetadataEntry>
    {
        private readonly byte[] data;

        public MetadataEntry(MetadataKey key, TagDataType type, byte[] data)
            : this(key.Section, key.TagId, type, data)
        {
        }

        public MetadataEntry(MetadataSection section, ushort tagId, TagDataType type, byte[] data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            Section = section;
            TagId = tagId;
            Type = type;
            this.data = (byte[])data.Clone();
        }

        public int LengthInBytes => data.Length;

        public MetadataSection Section { get; }

        public ushort TagId { get; }

        public TagDataType Type { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                return string.Format("{0}, Tag# {1} (0x{1:X}), {2}", Section, TagId, Type);
            }
        }

        public byte[] GetData()
        {
            return (byte[])data.Clone();
        }

        public byte[] GetDataReadOnly()
        {
            return data;
        }

#if PDN_3_5_X
        public System.Drawing.Imaging.PropertyItem TryCreateGdipPropertyItem()
        {
            // Skip the Interoperability IFD because GDI+ does not support it.
            // https://docs.microsoft.com/en-us/windows/desktop/gdiplus/-gdiplus-constant-image-property-tag-constants
            if (!TagDataTypeUtil.IsKnownToGDIPlus(Type) || Section == MetadataSection.Interop)
            {
                return null;
            }

            System.Drawing.Imaging.PropertyItem propertyItem = PaintDotNet.SystemLayer.PdnGraphics.CreatePropertyItem();

            propertyItem.Id = TagId;
            propertyItem.Type = (short)Type;
            propertyItem.Len = data.Length;
            propertyItem.Value = (byte[])data.Clone();

            return propertyItem;
        }
#else
        public PaintDotNet.Imaging.ExifPropertyItem CreateExifPropertyItem()
        {
            PaintDotNet.Imaging.ExifSection exifSection;
            switch (Section)
            {
                case MetadataSection.Image:
                    exifSection = PaintDotNet.Imaging.ExifSection.Image;
                    break;
                case MetadataSection.Exif:
                    exifSection = PaintDotNet.Imaging.ExifSection.Photo;
                    break;
                case MetadataSection.Gps:
                    exifSection = PaintDotNet.Imaging.ExifSection.GpsInfo;
                    break;
                case MetadataSection.Interop:
                    exifSection = PaintDotNet.Imaging.ExifSection.Interop;
                    break;
                default:
                    throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                                                                      "Unexpected {0} type: {1}",
                                                                      nameof(MetadataSection),
                                                                      (int)Section));
            }

            return new PaintDotNet.Imaging.ExifPropertyItem(exifSection,
                                                            TagId,
                                                            new PaintDotNet.Imaging.ExifValue((PaintDotNet.Imaging.ExifValueType)Type,
                                                                                              (byte[])data.Clone()));
        }
#endif

        public override bool Equals(object obj)
        {
            if (obj is MetadataEntry entry)
            {
                return Equals(entry);
            }

            return false;
        }

        public bool Equals(MetadataEntry other)
        {
            if (other is null)
            {
                return false;
            }

            return Section == other.Section;
        }

        public override int GetHashCode()
        {
            int hashCode = -2103575766;

            unchecked
            {
                hashCode = hashCode * -1521134295 + Section.GetHashCode();
                hashCode = hashCode * -1521134295 + TagId.GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(MetadataEntry left, MetadataEntry right)
        {
            return EqualityComparer<MetadataEntry>.Default.Equals(left, right);
        }

        public static bool operator !=(MetadataEntry left, MetadataEntry right)
        {
            return !(left == right);
        }
    }
}
