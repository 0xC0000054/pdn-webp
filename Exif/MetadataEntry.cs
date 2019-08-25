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
using System.Drawing.Imaging;

namespace WebPFileType.Exif
{
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

        public byte[] GetData()
        {
            return (byte[])data.Clone();
        }

        public byte[] GetDataReadOnly()
        {
            return data;
        }

        public PropertyItem TryCreateGdipPropertyItem()
        {
            // Skip the Interoperability IFD because GDI+ does not support it.
            // https://docs.microsoft.com/en-us/windows/desktop/gdiplus/-gdiplus-constant-image-property-tag-constants
            if (!TagDataTypeUtil.IsKnownToGDIPlus(Type) || Section == MetadataSection.Interop)
            {
                return null;
            }

            PropertyItem propertyItem = PaintDotNet.SystemLayer.PdnGraphics.CreatePropertyItem();

            propertyItem.Id = TagId;
            propertyItem.Type = (short)Type;
            propertyItem.Len = data.Length;
            propertyItem.Value = (byte[])data.Clone();

            return propertyItem;
        }

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
