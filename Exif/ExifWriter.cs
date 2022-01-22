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

using PaintDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebPFileType.Exif
{
    internal sealed class ExifWriter
    {
        private readonly Dictionary<MetadataSection, Dictionary<ushort, MetadataEntry>> metadata;

        private const int FirstIFDOffset = 8;

        public ExifWriter(Document doc, IDictionary<MetadataKey, MetadataEntry> entries, ExifColorSpace exifColorSpace)
        {
            metadata = CreateTagDictionary(doc, entries, exifColorSpace);
        }

        public byte[] CreateExifBlob()
        {
            IFDInfo ifdInfo = BuildIFDEntries();
            Dictionary<MetadataSection, IFDEntryInfo> ifdEntries = ifdInfo.IFDEntries;

            byte[] exifBytes = new byte[checked((int)ifdInfo.EXIFDataLength)];

            using (MemoryStream stream = new MemoryStream(exifBytes))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                IFDEntryInfo imageInfo = ifdEntries[MetadataSection.Image];
                IFDEntryInfo exifInfo = ifdEntries[MetadataSection.Exif];

                writer.Write(TiffConstants.LittleEndianByteOrderMarker);
                writer.Write(TiffConstants.Signature);
                writer.Write((uint)imageInfo.StartOffset);

                WriteDirectory(writer, metadata[MetadataSection.Image], imageInfo.IFDEntries, imageInfo.StartOffset);
                WriteDirectory(writer, metadata[MetadataSection.Exif], exifInfo.IFDEntries, exifInfo.StartOffset);

                if (ifdEntries.TryGetValue(MetadataSection.Interop, out IFDEntryInfo interopInfo))
                {
                    WriteDirectory(writer, metadata[MetadataSection.Interop], interopInfo.IFDEntries, interopInfo.StartOffset);
                }

                if (ifdEntries.TryGetValue(MetadataSection.Gps, out IFDEntryInfo gpsInfo))
                {
                    WriteDirectory(writer, metadata[MetadataSection.Gps], gpsInfo.IFDEntries, gpsInfo.StartOffset);
                }
            }

            return exifBytes;
        }

        private static void WriteDirectory(BinaryWriter writer, Dictionary<ushort, MetadataEntry> tags,  List<IFDEntry> entries, long ifdOffset)
        {
            writer.BaseStream.Position = ifdOffset;

            long nextIFDPointerOffset = ifdOffset + sizeof(ushort) + ((long)entries.Count * IFDEntry.SizeOf);

            writer.Write((ushort)entries.Count);

            foreach (IFDEntry entry in entries.OrderBy(e => e.Tag))
            {
                entry.Write(writer);

                if (!TagDataTypeUtil.ValueFitsInOffsetField(entry.Type, entry.Count))
                {
                    long oldPosition = writer.BaseStream.Position;

                    writer.BaseStream.Position = entry.Offset;

                    writer.Write(tags[entry.Tag].GetDataReadOnly());

                    writer.BaseStream.Position = oldPosition;
                }
            }

            writer.BaseStream.Position = nextIFDPointerOffset;
            // There is only one IFD in this directory.
            writer.Write(0);
        }

        private IFDInfo BuildIFDEntries()
        {
            Dictionary<ushort, MetadataEntry> imageMetadata = metadata[MetadataSection.Image];
            Dictionary<ushort, MetadataEntry> exifMetadata = metadata[MetadataSection.Exif];

            // Add placeholders for the sub-IFD tags.
            imageMetadata.Add(
                MetadataKeys.Image.ExifTag.TagId,
                new MetadataEntry(MetadataKeys.Image.ExifTag,
                                  TagDataType.Long,
                                  new byte[sizeof(uint)]));

            if (metadata.ContainsKey(MetadataSection.Gps))
            {
                imageMetadata.Add(
                MetadataKeys.Image.GPSTag.TagId,
                new MetadataEntry(MetadataKeys.Image.GPSTag,
                                  TagDataType.Long,
                                  new byte[sizeof(uint)]));
            }

            if (metadata.ContainsKey(MetadataSection.Interop))
            {
                exifMetadata.Add(
                    MetadataKeys.Exif.InteroperabilityTag.TagId,
                    new MetadataEntry(MetadataKeys.Exif.InteroperabilityTag,
                                      TagDataType.Long,
                                      new byte[sizeof(uint)]));
            }

            return CalculateSectionOffsets();
        }

        private IFDInfo CalculateSectionOffsets()
        {
            IFDEntryInfo imageIFDInfo = CreateIFDList(metadata[MetadataSection.Image], FirstIFDOffset);
            IFDEntryInfo exifIFDInfo = CreateIFDList(metadata[MetadataSection.Exif], imageIFDInfo.NextAvailableOffset);
            IFDEntryInfo interopIFDInfo = null;
            IFDEntryInfo gpsIFDInfo = null;

            UpdateSubIFDOffset(ref imageIFDInfo, MetadataKeys.Image.ExifTag.TagId, (uint)exifIFDInfo.StartOffset);

            if (metadata.TryGetValue(MetadataSection.Interop, out Dictionary<ushort, MetadataEntry> interopSection))
            {
                interopIFDInfo = CreateIFDList(interopSection, exifIFDInfo.NextAvailableOffset);

                UpdateSubIFDOffset(ref exifIFDInfo, MetadataKeys.Exif.InteroperabilityTag.TagId, (uint)interopIFDInfo.StartOffset);
            }

            if (metadata.TryGetValue(MetadataSection.Gps, out Dictionary<ushort, MetadataEntry> gpsSection))
            {
                long startOffset = interopIFDInfo?.NextAvailableOffset ?? exifIFDInfo.NextAvailableOffset;
                gpsIFDInfo = CreateIFDList(gpsSection, startOffset);

                UpdateSubIFDOffset(ref imageIFDInfo, MetadataKeys.Image.GPSTag.TagId, (uint)gpsIFDInfo.StartOffset);
            }

            return CreateIFDInfo(imageIFDInfo, exifIFDInfo, interopIFDInfo, gpsIFDInfo);
        }

        private static void UpdateSubIFDOffset(ref IFDEntryInfo ifdInfo, ushort tagId, uint newOffset)
        {
            int index = ifdInfo.IFDEntries.FindIndex(i => i.Tag == tagId);

            if (index != -1)
            {
                ifdInfo.IFDEntries[index] = new IFDEntry(tagId, TagDataType.Long, 1, newOffset);
            }
        }

        private static IFDInfo CreateIFDInfo(
            IFDEntryInfo imageIFDInfo,
            IFDEntryInfo exifIFDInfo,
            IFDEntryInfo interopIFDInfo,
            IFDEntryInfo gpsIFDInfo)
        {
            Dictionary<MetadataSection, IFDEntryInfo> entries = new Dictionary<MetadataSection, IFDEntryInfo>
            {
                { MetadataSection.Image, imageIFDInfo },
                { MetadataSection.Exif, exifIFDInfo }
            };

            long dataLength = exifIFDInfo.NextAvailableOffset;

            if (interopIFDInfo != null)
            {
                entries.Add(MetadataSection.Interop, interopIFDInfo);
                dataLength = interopIFDInfo.NextAvailableOffset;
            }

            if (gpsIFDInfo != null)
            {
                entries.Add(MetadataSection.Gps, gpsIFDInfo);
                dataLength = gpsIFDInfo.NextAvailableOffset;
            }

            return new IFDInfo(entries, dataLength);
        }

        private static IFDEntryInfo CreateIFDList(Dictionary<ushort, MetadataEntry> tags, long startOffset)
        {
            List<IFDEntry> ifdEntries = new List<IFDEntry>(tags.Count);

            // Leave room for the tag count, tags and next IFD offset.
            long ifdDataOffset = startOffset + sizeof(ushort) + ((long)tags.Count * IFDEntry.SizeOf) + sizeof(uint);

            foreach (KeyValuePair<ushort, MetadataEntry> item in tags.OrderBy(i => i.Key))
            {
                MetadataEntry entry = item.Value;

                uint count;
                switch (entry.Type)
                {
                    case TagDataType.Byte:
                    case TagDataType.Ascii:
                    case TagDataType.SByte:
                    case TagDataType.Undefined:
                        count = (uint)entry.LengthInBytes;
                        break;
                    case TagDataType.Short:
                    case TagDataType.SShort:
                        count = (uint)entry.LengthInBytes / 2;
                        break;
                    case TagDataType.Long:
                    case TagDataType.SLong:
                    case TagDataType.Float:
                        count = (uint)entry.LengthInBytes / 4;
                        break;
                    case TagDataType.Rational:
                    case TagDataType.SRational:
                    case TagDataType.Double:
                        count = (uint)entry.LengthInBytes / 8;
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected tag type.");
                }

                if (TagDataTypeUtil.ValueFitsInOffsetField(entry.Type, count))
                {
                    uint packedOffset = 0;

                    // Some applications may write EXIF fields with a count of zero.
                    // See https://github.com/0xC0000054/pdn-webp/issues/6.
                    if (count > 0)
                    {
                        byte[] data = entry.GetDataReadOnly();

                        // The data is always in little-endian byte order.
                        switch (data.Length)
                        {
                            case 1:
                                packedOffset |= data[0];
                                break;
                            case 2:
                                packedOffset |= data[0];
                                packedOffset |= (uint)data[1] << 8;
                                break;
                            case 3:
                                packedOffset |= data[0];
                                packedOffset |= (uint)data[1] << 8;
                                packedOffset |= (uint)data[2] << 16;
                                break;
                            case 4:
                                packedOffset |= data[0];
                                packedOffset |= (uint)data[1] << 8;
                                packedOffset |= (uint)data[2] << 16;
                                packedOffset |= (uint)data[3] << 24;
                                break;
                            default:
                                throw new InvalidOperationException("data.Length must be in the range of [1-4].");
                        }
                    }

                    ifdEntries.Add(new IFDEntry(entry.TagId, entry.Type, count, packedOffset));
                }
                else
                {
                    ifdEntries.Add(new IFDEntry(entry.TagId, entry.Type, count, (uint)ifdDataOffset));
                    ifdDataOffset += entry.LengthInBytes;

                    // The IFD offsets must begin on a WORD boundary.
                    if ((ifdDataOffset & 1) == 1)
                    {
                        ifdDataOffset++;
                    }
                }
            }

            return new IFDEntryInfo(ifdEntries, startOffset, ifdDataOffset);
        }

        private static Dictionary<MetadataSection, Dictionary<ushort, MetadataEntry>> CreateTagDictionary(
            Document doc,
            IDictionary<MetadataKey, MetadataEntry> entries,
            ExifColorSpace exifColorSpace)
        {
            Dictionary<MetadataSection, Dictionary<ushort, MetadataEntry>> metadataEntries = new Dictionary<MetadataSection, Dictionary<ushort, MetadataEntry>>
            {
                {
                    MetadataSection.Image,
                    new Dictionary<ushort, MetadataEntry>
                    {
                        {
                            MetadataKeys.Image.Orientation.TagId,
                            new MetadataEntry(MetadataKeys.Image.Orientation,
                                              TagDataType.Short,
                                              MetadataHelpers.EncodeShort(TiffConstants.Orientation.TopLeft))
                        }
                    }
                },
                {
                    MetadataSection.Exif,
                    new Dictionary<ushort, MetadataEntry>
                    {
                        {
                            MetadataKeys.Exif.ColorSpace.TagId,
                            new MetadataEntry(MetadataKeys.Exif.ColorSpace,
                                              TagDataType.Short,
                                              MetadataHelpers.EncodeShort((ushort)exifColorSpace))
                        }
                    }
                }
            };

            // Add the image size tags.
            if (IsUncompressedImage(entries))
            {
                Dictionary<ushort, MetadataEntry> imageSection = metadataEntries[MetadataSection.Image];
                imageSection.Add(MetadataKeys.Image.ImageWidth.TagId,
                                new MetadataEntry(MetadataKeys.Image.ImageWidth,
                                                  TagDataType.Long,
                                                  MetadataHelpers.EncodeLong((uint)doc.Width)));
                imageSection.Add(MetadataKeys.Image.ImageLength.TagId,
                                new MetadataEntry(MetadataKeys.Image.ImageLength,
                                                  TagDataType.Long,
                                                  MetadataHelpers.EncodeLong((uint)doc.Height)));

                entries.Remove(MetadataKeys.Image.ImageWidth);
                entries.Remove(MetadataKeys.Image.ImageLength);
                // These tags should not be included in uncompressed images.
                entries.Remove(MetadataKeys.Exif.PixelXDimension);
                entries.Remove(MetadataKeys.Exif.PixelYDimension);
            }
            else
            {
                Dictionary<ushort, MetadataEntry> exifSection = metadataEntries[MetadataSection.Exif];
                exifSection.Add(MetadataKeys.Exif.PixelXDimension.TagId,
                                new MetadataEntry(MetadataKeys.Exif.PixelXDimension,
                                                  TagDataType.Long,
                                                  MetadataHelpers.EncodeLong((uint)doc.Width)));
                exifSection.Add(MetadataKeys.Exif.PixelYDimension.TagId,
                                new MetadataEntry(MetadataKeys.Exif.PixelYDimension,
                                                  TagDataType.Long,
                                                  MetadataHelpers.EncodeLong((uint)doc.Height)));

                entries.Remove(MetadataKeys.Exif.PixelXDimension);
                entries.Remove(MetadataKeys.Exif.PixelYDimension);
                // These tags should not be included in compressed images.
                entries.Remove(MetadataKeys.Image.ImageWidth);
                entries.Remove(MetadataKeys.Image.ImageLength);
            }

            foreach (KeyValuePair<MetadataKey, MetadataEntry> kvp in entries)
            {
                MetadataEntry entry = kvp.Value;

                MetadataSection section = entry.Section;

                if (section == MetadataSection.Image && !ExifTagHelper.CanWriteImageSectionTag(entry.TagId))
                {
                    continue;
                }

                if (metadataEntries.TryGetValue(section, out Dictionary<ushort, MetadataEntry> values))
                {
                    if (!values.ContainsKey(entry.TagId))
                    {
                        values.Add(entry.TagId, entry);
                    }
                }
                else
                {
                    metadataEntries.Add(section, new Dictionary<ushort, MetadataEntry>
                    {
                        { entry.TagId, entry }
                    });
                }
            }

            AddVersionEntries(ref metadataEntries);

            return metadataEntries;
        }

        private static bool IsUncompressedImage(IDictionary<MetadataKey, MetadataEntry> entries)
        {
            return entries.ContainsKey(MetadataKeys.Image.ImageWidth);
        }

        private static void AddVersionEntries(ref Dictionary<MetadataSection, Dictionary<ushort, MetadataEntry>> metadataEntries)
        {
            if (metadataEntries.TryGetValue(MetadataSection.Exif, out Dictionary<ushort, MetadataEntry> exifItems))
            {
                if (!exifItems.ContainsKey(MetadataKeys.Exif.ExifVersion.TagId))
                {
                    exifItems.Add(
                        MetadataKeys.Exif.ExifVersion.TagId,
                        new MetadataEntry(MetadataKeys.Exif.ExifVersion,
                                          TagDataType.Undefined,
                                          new byte[] { (byte)'0', (byte)'2', (byte)'3', (byte)'0' }));
                }
            }

            if (metadataEntries.TryGetValue(MetadataSection.Gps, out Dictionary<ushort, MetadataEntry> gpsItems))
            {
                if (!gpsItems.ContainsKey(MetadataKeys.Gps.GPSVersionID.TagId))
                {
                    gpsItems.Add(
                        MetadataKeys.Gps.GPSVersionID.TagId,
                        new MetadataEntry(MetadataKeys.Gps.GPSVersionID,
                                          TagDataType.Byte,
                                          new byte[] { 2, 3, 0, 0 }));
                }
            }
        }

        private sealed class IFDEntryInfo
        {
            public IFDEntryInfo(List<IFDEntry> ifdEntries, long startOffset, long nextAvailableOffset)
            {
                if (ifdEntries is null)
                {
                    throw new ArgumentNullException(nameof(ifdEntries));
                }

                IFDEntries = ifdEntries;
                StartOffset = startOffset;
                NextAvailableOffset = nextAvailableOffset;
            }

            public List<IFDEntry> IFDEntries { get; }

            public long StartOffset { get; }

            public long NextAvailableOffset { get; }
        }

        private sealed class IFDInfo
        {
            public IFDInfo(Dictionary<MetadataSection, IFDEntryInfo> entries, long exifDataLength)
            {
                if (entries is null)
                {
                    throw new ArgumentNullException(nameof(entries));
                }

                IFDEntries = entries;
                EXIFDataLength = exifDataLength;
            }

            public Dictionary<MetadataSection, IFDEntryInfo> IFDEntries { get; }

            public long EXIFDataLength { get; }
        }
    }
}
