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
using System.Globalization;
using System.IO;
using System.Text;

namespace WebPFileType.Exif
{
    internal static class ExifParser
    {
        private const ushort TiffSignature = 42;

        private enum TagDataType : ushort
        {
            Byte = 1,
            Ascii = 2,
            Short = 3,
            Long = 4,
            Rational = 5,
            SByte = 6,
            Undefined = 7,
            SShort = 8,
            SLong = 9,
            SRational = 10,
            Float = 11,
            Double = 12,
            IFD = 13
        }

        /// <summary>
        /// Parses the EXIF data into a collection of properties.
        /// </summary>
        /// <param name="exifBytes">The EXIF bytes.</param>
        /// <returns>
        /// A collection containing the EXIF properties.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="exifBytes"/> is null.</exception>
        internal static ExifValueCollection Parse(byte[] exifBytes)
        {
            if (exifBytes == null)
            {
                throw new ArgumentNullException(nameof(exifBytes));
            }

            List<PropertyItem> propertyItems = new List<PropertyItem>();

            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(exifBytes);

                Endianess? byteOrder = TryDetectTiffByteOrder(stream);

                if (byteOrder.HasValue)
                {
                    using (EndianBinaryReader reader = new EndianBinaryReader(stream, byteOrder.Value))
                    {
                        stream = null;

                        ushort signature = reader.ReadUInt16();

                        if (signature == TiffSignature)
                        {
                            uint ifdOffset = reader.ReadUInt32();

                            List<IFDEntry> entries = ParseDirectories(reader, ifdOffset);

                            propertyItems.AddRange(ConvertIFDEntriesToPropertyItems(reader, entries));
                        }
                    }
                }
            }
            catch (EndOfStreamException)
            {
            }
            finally
            {
                stream?.Dispose();
            }

            return new ExifValueCollection(propertyItems);
        }

        private static Endianess? TryDetectTiffByteOrder(Stream stream)
        {
            int byte1 = stream.ReadByte();
            if (byte1 == -1)
            {
                return null;
            }

            int byte2 = stream.ReadByte();
            if (byte2 == -1)
            {
                return null;
            }

            if (byte1 == 0x4D && byte2 == 0x4D)
            {
                return Endianess.Big;
            }
            else if (byte1 == 0x49 && byte2 == 0x49)
            {
                return Endianess.Little;
            }
            else
            {
                return null;
            }
        }

        private static ICollection<PropertyItem> ConvertIFDEntriesToPropertyItems(EndianBinaryReader reader, List<IFDEntry> entries)
        {
            List<PropertyItem> propertyItems = new List<PropertyItem>(entries.Count);
            bool swapNumberByteOrder = reader.Endianess == Endianess.Big;

            for (int i = 0; i < entries.Count; i++)
            {
                IFDEntry entry = entries[i];

                if (!TagDataTypeUtil.IsKnownToGDIPlus(entry.Type))
                {
                    continue;
                }

                byte[] propertyData;
                if (entry.OffsetFieldContainsValue)
                {
                    propertyData = entry.GetValueBytesFromOffset();
                    if (propertyData == null)
                    {
                        continue;
                    }
                }
                else
                {
                    long bytesToRead = entry.Count * TagDataTypeUtil.GetSizeInBytes(entry.Type);

                    // Skip any tags that are empty or larger than 2 GB.
                    if (bytesToRead == 0 || bytesToRead > int.MaxValue)
                    {
                        continue;
                    }

                    uint offset = entry.Offset;

                    if ((offset + bytesToRead) > reader.Length)
                    {
                        continue;
                    }

                    reader.Position = offset;

                    propertyData = reader.ReadBytes((int)bytesToRead);

                    if (swapNumberByteOrder)
                    {
                        // GDI+ converts all multi-byte numbers to little-endian when creating a PropertyItem.
                        switch (entry.Type)
                        {
                            case TagDataType.Short:
                            case TagDataType.SShort:
                                propertyData = SwapShortArrayToLittleEndian(propertyData, entry.Count);
                                break;
                            case TagDataType.Long:
                            case TagDataType.SLong:
                            case TagDataType.Float:
                                propertyData = SwapLongArrayToLittleEndian(propertyData, entry.Count);
                                break;
                            case TagDataType.Rational:
                            case TagDataType.SRational:
                                propertyData = SwapRationalArrayToLittleEndian(propertyData, entry.Count);
                                break;
                            case TagDataType.Double:
                                propertyData = SwapDoubleArrayToLittleEndian(propertyData, entry.Count);
                                break;
                            case TagDataType.Byte:
                            case TagDataType.Ascii:
                            case TagDataType.Undefined:
                            case TagDataType.SByte:
                            default:
                                break;
                        }
                    }
                }

                PropertyItem propertyItem = PaintDotNet.SystemLayer.PdnGraphics.CreatePropertyItem();
                propertyItem.Id = entry.Tag;
                propertyItem.Type = (short)entry.Type;
                propertyItem.Len = propertyData.Length;
                propertyItem.Value = (byte[])propertyData.Clone();

                propertyItems.Add(propertyItem);
            }

            return propertyItems;
        }

        private static List<IFDEntry> ParseDirectories(EndianBinaryReader reader, uint firstIFDOffset)
        {
            List<IFDEntry> items = new List<IFDEntry>();

            bool foundExif = false;
            bool foundGps = false;

            Queue<uint> ifdOffsets = new Queue<uint>();
            ifdOffsets.Enqueue(firstIFDOffset);

            while (ifdOffsets.Count > 0)
            {
                uint offset = ifdOffsets.Dequeue();

                if (offset >= reader.Length)
                {
                    continue;
                }

                reader.Position = offset;

                ushort count = reader.ReadUInt16();
                if (count == 0)
                {
                    continue;
                }

                items.Capacity += count;

                for (int i = 0; i < count; i++)
                {
                    IFDEntry entry = new IFDEntry(reader);

                    switch (entry.Tag)
                    {
                        case TiffTags.ExifIFD:
                            if (!foundExif)
                            {
                                foundExif = true;
                                ifdOffsets.Enqueue(entry.Offset);
                            }
                            break;
                        case TiffTags.GpsIFD:
                            if (!foundGps)
                            {
                                foundGps = true;
                                ifdOffsets.Enqueue(entry.Offset);
                            }
                            break;
                        case TiffTags.InteropIFD:
                            // Skip the Interoperability IFD because GDI+ does not support it.
                            // https://docs.microsoft.com/en-us/windows/desktop/gdiplus/-gdiplus-constant-image-property-tag-constants
                            break;
                        case TiffTags.StripOffsets:
                        case TiffTags.StripByteCounts:
                        case TiffTags.SubIFDs:
                        case TiffTags.ThumbnailOffset:
                        case TiffTags.ThumbnailLength:
                            // Skip the thumbnail and/or preview images.
                            // The StripOffsets and StripByteCounts tags are used to store a preview image in some formats.
                            // The SubIFDs tag is used to store thumbnails in TIFF and for storing other data in some camera formats.
                            //
                            // Note that some cameras will also store a thumbnail as part of their private data in the EXIF MakerNote tag.
                            // The EXIF MakerNote tag is treated as an opaque blob, so those thumbnails will be preserved.
                            break;
                        default:
                            items.Add(entry);
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine(entry.ToString());
                }
            }

            return items;
        }

        private static unsafe byte[] SwapShortArrayToLittleEndian(byte[] values, uint count)
        {
            fixed (byte* pBytes = values)
            {
                ushort* ptr = (ushort*)pBytes;
                ushort* ptrEnd = ptr + count;

                while (ptr < ptrEnd)
                {
                    *ptr = EndianUtil.Swap(*ptr);
                    ptr++;
                }
            }

            return values;
        }

        private static unsafe byte[] SwapLongArrayToLittleEndian(byte[] values, uint count)
        {
            fixed (byte* pBytes = values)
            {
                uint* ptr = (uint*)pBytes;
                uint* ptrEnd = ptr + count;

                while (ptr < ptrEnd)
                {
                    *ptr = EndianUtil.Swap(*ptr);
                    ptr++;
                }
            }

            return values;
        }

        private static unsafe byte[] SwapRationalArrayToLittleEndian(byte[] values, uint count)
        {
            // A rational value consists of two 4-byte values, a numerator and a denominator.
            long itemCount = count * 2;

            fixed (byte* pBytes = values)
            {
                uint* ptr = (uint*)pBytes;
                uint* ptrEnd = ptr + itemCount;

                while (ptr < ptrEnd)
                {
                    *ptr = EndianUtil.Swap(*ptr);
                    ptr++;
                }
            }

            return values;
        }

        private static unsafe byte[] SwapDoubleArrayToLittleEndian(byte[] values, uint count)
        {
            fixed (byte* pBytes = values)
            {
                ulong* ptr = (ulong*)pBytes;
                ulong* ptrEnd = ptr + count;

                while (ptr < ptrEnd)
                {
                    *ptr = EndianUtil.Swap(*ptr);
                    ptr++;
                }
            }

            return values;
        }

        private readonly struct IFDEntry
        {
#pragma warning disable IDE0032 // Use auto property
            private readonly ushort tag;
            private readonly TagDataType type;
            private readonly uint count;
            private readonly uint offset;
            private readonly bool offsetIsBigEndian;
#pragma warning restore IDE0032 // Use auto property

            public IFDEntry(EndianBinaryReader reader)
            {
                tag = reader.ReadUInt16();
                type = (TagDataType)reader.ReadUInt16();
                count = reader.ReadUInt32();
                offset = reader.ReadUInt32();
                offsetIsBigEndian = reader.Endianess == Endianess.Big;
            }

            public ushort Tag => tag;

            public TagDataType Type => type;

            public uint Count => count;

            public uint Offset => offset;

            public bool OffsetFieldContainsValue
            {
                get
                {
                    switch (type)
                    {
                        case TagDataType.Byte:
                        case TagDataType.Ascii:
                        case TagDataType.Undefined:
                        case TagDataType.SByte:
                            return count <= 4;
                        case TagDataType.Short:
                        case TagDataType.SShort:
                            return count <= 2;
                        case TagDataType.Long:
                        case TagDataType.SLong:
                        case TagDataType.Float:
                        case TagDataType.IFD:
                            return count == 1;
                        case TagDataType.Rational:
                        case TagDataType.SRational:
                        case TagDataType.Double:
                        default:
                            return false;
                    }
                }
            }

            public unsafe byte[] GetValueBytesFromOffset()
            {
                if (!OffsetFieldContainsValue)
                {
                    return null;
                }

                // GDI+ always stores data in little-endian byte order.
                byte[] bytes;
                if (type == TagDataType.Byte ||
                    type == TagDataType.Ascii ||
                    type == TagDataType.SByte ||
                    type == TagDataType.Undefined)
                {
                    bytes = new byte[count];

                    if (offsetIsBigEndian)
                    {
                        switch (count)
                        {
                            case 1:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                break;
                            case 2:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                break;
                            case 3:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 8) & 0x000000ff);
                                break;
                            case 4:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[3] = (byte)(offset & 0x000000ff);
                                break;
                        }
                    }
                    else
                    {
                        switch (count)
                        {
                            case 1:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                break;
                            case 2:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                break;
                            case 3:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 16) & 0x000000ff);
                                break;
                            case 4:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[3] = (byte)((offset >> 24) & 0x000000ff);
                                break;
                        }
                    }
                }
                else if (type == TagDataType.Short || type == TagDataType.SShort)
                {
                    int byteArrayLength = unchecked((int)count) * sizeof(ushort);
                    bytes = new byte[byteArrayLength];

                    fixed (byte* ptr = bytes)
                    {
                        ushort* ushortPtr = (ushort*)ptr;

                        if (offsetIsBigEndian)
                        {
                            switch (count)
                            {
                                case 1:
                                    ushortPtr[0] = (ushort)((offset >> 16) & 0x0000ffff);
                                    break;
                                case 2:
                                    ushortPtr[0] = (ushort)((offset >> 16) & 0x0000ffff);
                                    ushortPtr[1] = (ushort)(offset & 0x0000ffff);
                                    break;
                            }
                        }
                        else
                        {
                            switch (count)
                            {
                                case 1:
                                    ushortPtr[0] = (ushort)(offset & 0x0000ffff);
                                    break;
                                case 2:
                                    ushortPtr[0] = (ushort)(offset & 0x0000ffff);
                                    ushortPtr[1] = (ushort)((offset >> 16) & 0x0000ffff);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    bytes = new byte[4];

                    if (offsetIsBigEndian)
                    {
                        bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                        bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                        bytes[2] = (byte)((offset >> 8) & 0x000000ff);
                        bytes[3] = (byte)(offset & 0x000000ff);
                    }
                    else
                    {
                        bytes[0] = (byte)(offset & 0x000000ff);
                        bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                        bytes[2] = (byte)((offset >> 16) & 0x000000ff);
                        bytes[3] = (byte)((offset >> 24) & 0x000000ff);
                    }
                }

                return bytes;
            }

            public override string ToString()
            {
                if (OffsetFieldContainsValue)
                {
                    return string.Format("Tag={0}, Type={1}, Count={2}, Value={3}",
                                         tag.ToString(CultureInfo.InvariantCulture),
                                         type.ToString(),
                                         count.ToString(CultureInfo.InvariantCulture),
                                         GetValueStringFromOffset());
                }
                else
                {
                    return string.Format("Tag={0}, Type={1}, Count={2}, Offset=0x{3}",
                                         tag.ToString(CultureInfo.InvariantCulture),
                                         type.ToString(),
                                         count.ToString(CultureInfo.InvariantCulture),
                                         offset.ToString("X", CultureInfo.InvariantCulture));
                }
            }

            private string GetValueStringFromOffset()
            {
                string valueString;

                int typeSizeInBytes = TagDataTypeUtil.GetSizeInBytes(type);

                if (typeSizeInBytes == 1)
                {
                    byte[] bytes = new byte[count];

                    if (offsetIsBigEndian)
                    {
                        switch (count)
                        {
                            case 1:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                break;
                            case 2:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                break;
                            case 3:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 8) & 0x000000ff);
                                break;
                            case 4:
                                bytes[0] = (byte)((offset >> 24) & 0x000000ff);
                                bytes[1] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[3] = (byte)(offset & 0x000000ff);
                                break;
                        }
                    }
                    else
                    {
                        switch (count)
                        {
                            case 1:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                break;
                            case 2:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                break;
                            case 3:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 16) & 0x000000ff);
                                break;
                            case 4:
                                bytes[0] = (byte)(offset & 0x000000ff);
                                bytes[1] = (byte)((offset >> 8) & 0x000000ff);
                                bytes[2] = (byte)((offset >> 16) & 0x000000ff);
                                bytes[3] = (byte)((offset >> 24) & 0x000000ff);
                                break;
                        }
                    }

                    if (type == TagDataType.Ascii)
                    {
                        valueString = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
                    }
                    else if (count == 1)
                    {
                        valueString = bytes[0].ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();

                        uint lastItemIndex = count - 1;

                        for (int i = 0; i < count; i++)
                        {
                            builder.Append(bytes[i].ToString(CultureInfo.InvariantCulture));

                            if (i < lastItemIndex)
                            {
                                builder.Append(",");
                            }
                        }

                        valueString = builder.ToString();
                    }
                }
                else if (typeSizeInBytes == 2)
                {
                    ushort[] values = new ushort[count];
                    if (offsetIsBigEndian)
                    {
                        switch (count)
                        {
                            case 1:
                                values[0] = (ushort)((offset >> 16) & 0x0000ffff);
                                break;
                            case 2:
                                values[0] = (ushort)((offset >> 16) & 0x0000ffff);
                                values[1] = (ushort)(offset & 0x0000ffff);
                                break;
                        }
                    }
                    else
                    {
                        switch (count)
                        {
                            case 1:
                                values[0] = (ushort)(offset & 0x0000ffff);
                                break;
                            case 2:
                                values[0] = (ushort)(offset & 0x0000ffff);
                                values[1] = (ushort)((offset >> 16) & 0x0000ffff);
                                break;
                        }
                    }

                    if (count == 1)
                    {
                        switch (type)
                        {
                            case TagDataType.SShort:
                                valueString = ((short)values[0]).ToString(CultureInfo.InvariantCulture);
                                break;
                            case TagDataType.Short:
                            default:
                                valueString = values[0].ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case TagDataType.SShort:
                                valueString = ((short)values[0]).ToString(CultureInfo.InvariantCulture) + "," +
                                              ((short)values[1]).ToString(CultureInfo.InvariantCulture);
                                break;
                            case TagDataType.Short:
                            default:
                                valueString = values[0].ToString(CultureInfo.InvariantCulture) + "," +
                                              values[1].ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                    }
                }
                else
                {
                    valueString = offset.ToString(CultureInfo.InvariantCulture);
                }

                return valueString;
            }
        }

        private static class TagDataTypeUtil
        {
            /// <summary>
            /// Determines whether the <see cref="TagDataType"/> is known to GDI+.
            /// </summary>
            /// <param name="type">The tag type.</param>
            /// <returns>
            /// <see langword="true"/> if the tag type is known to GDI+; otherwise, <see langword="false"/>.
            /// </returns>
            public static bool IsKnownToGDIPlus(TagDataType type)
            {
                switch (type)
                {
                    case TagDataType.Byte:
                    case TagDataType.Ascii:
                    case TagDataType.Short:
                    case TagDataType.Long:
                    case TagDataType.Rational:
                    case TagDataType.SByte:
                    case TagDataType.Undefined:
                    case TagDataType.SShort:
                    case TagDataType.SLong:
                    case TagDataType.SRational:
                    case TagDataType.Float:
                    case TagDataType.Double:
                        return true;
                    default:
                        return false;
                }
            }

            /// <summary>
            /// Gets the size in bytes of a <see cref="TagDataType"/> value.
            /// </summary>
            /// <param name="type">The tag type.</param>
            /// <returns>
            /// The size of the value in bytes.
            /// </returns>
            public static int GetSizeInBytes(TagDataType type)
            {
                switch (type)
                {
                    case TagDataType.Byte:
                    case TagDataType.Ascii:
                    case TagDataType.Undefined:
                    case TagDataType.SByte:
                        return 1;
                    case TagDataType.Short:
                    case TagDataType.SShort:
                        return 2;
                    case TagDataType.Long:
                    case TagDataType.SLong:
                    case TagDataType.Float:
                    case TagDataType.IFD:
                        return 4;
                    case TagDataType.Rational:
                    case TagDataType.SRational:
                    case TagDataType.Double:
                        return 8;
                    default:
                        return 0;
                }
            }
        }

        private static class TiffTags
        {
            internal const ushort StripOffsets = 273;
            internal const ushort StripByteCounts = 279;
            internal const ushort SubIFDs = 330;
            internal const ushort ThumbnailOffset = 513;
            internal const ushort ThumbnailLength = 514;
            internal const ushort ExifIFD = 34665;
            internal const ushort GpsIFD = 34853;
            internal const ushort InteropIFD = 40965;
        }
    }
}
