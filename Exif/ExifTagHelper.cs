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

using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace WebPFileType.Exif
{
    internal static class ExifTagHelper
    {
        private static readonly HashSet<ushort> supportedTiffImageTagsForWriting = new HashSet<ushort>
        {
            // The tags related to storing offsets are included for reference,
            // but are not written to the EXIF blob.

            // Tags relating to image data structure
            256, // ImageWidth
            257, // ImageLength
            258, // BitsPerSample
            259, // Compression
            262, // PhotometricInterpretation
            274, // Orientation
            277, // SamplesPerPixel
            284, // PlanarConfiguration
            530, // YCbCrSubSampling
            531, // YCbCrPositioning
            282, // XResolution
            283, // YResolution
            296, // ResolutionUnit

            // Tags relating to recording offset
            //273, // StripOffsets
            //278, // RowsPerStrip
            //279, // StripByteCounts
            //513, // JPEGInterchangeFormat
            //514, // JPEGInterchangeFormatLength

            // Tags relating to image data characteristics
            301, // TransferFunction
            318, // WhitePoint
            319, // PrimaryChromaticities
            529, // YCbCrCoefficients
            532, // ReferenceBlackWhite

            // Other tags
            306, // DateTime
            270, // ImageDescription
            271, // Make
            272, // Model
            305, // Software
            315, // Artist
            33432 // Copyright
        };

        internal static bool CanWriteImageSectionTag(ushort tagId)
        {
            return supportedTiffImageTagsForWriting.Contains(tagId);
        }

        internal static MetadataSection GuessTagSection(PropertyItem propertyItem)
        {
            MetadataKey[] values;

            if (GusssTagSectionHelper.tiffAndExifTags.TryGetValue(propertyItem.Id, out values))
            {
                if (values.Length == 1)
                {
                    return values[0].Section;
                }

                IEnumerable<MetadataSection> sections = values.Select(v => v.Section);

                // Place items in the EXIF section, if possible.
                if (sections.Contains(MetadataSection.Exif))
                {
                    return MetadataSection.Exif;
                }
                else
                {
                    return MetadataSection.Image;
                }
            }
            else if (IsGpsTag(propertyItem))
            {
                return MetadataSection.Gps;
            }
            else if (IsInteroperabilityTag(propertyItem))
            {
                return MetadataSection.Interop;
            }
            else
            {
                return MetadataSection.Exif;
            }
        }

        private static bool IsGpsTag(PropertyItem propertyItem)
        {
            if (propertyItem.Id == 1)
            {
                // The tag number 1 is used by both the GPS IFD (GPSLatitudeRef) and the Interoperability IFD (InteroperabilityIndex).
                // The EXIF specification states that GPSLatitudeRef should be a 2 character ASCII field.

                return propertyItem.Type == (short)TagDataType.Ascii && propertyItem.Value.Length == 2;
            }
            else if (propertyItem.Id == 2)
            {
                // The tag number 2 is used by both the GPS IFD (GPSLatitude) and the Interoperability IFD (InteroperabilityVersion).
                // The EXIF specification states that GPSLatitude should be 3 rational numbers.

                return propertyItem.Type == (short)TagDataType.Rational && propertyItem.Value.Length == 24;
            }
            else
            {
                return propertyItem.Id >= 3 && propertyItem.Id <= 31;
            }
        }

        private static bool IsInteroperabilityTag(PropertyItem propertyItem)
        {
            if (propertyItem.Id == 1)
            {
                // The tag number 1 is used by both the GPS IFD (GPSLatitudeRef) and the Interoperability IFD (InteroperabilityIndex).
                // The EXIF specification states that InteroperabilityIndex should be a four character ASCII field.

                return propertyItem.Type == (short)TagDataType.Ascii && propertyItem.Value.Length == 4;
            }
            else if (propertyItem.Id == 2)
            {
                // The tag number 2 is used by both the GPS IFD (GPSLatitude) and the Interoperability IFD (InteroperabilityVersion).
                // The DCF specification states that InteroperabilityVersion should be a four byte field.
                switch ((TagDataType)propertyItem.Type)
                {
                    case TagDataType.Byte:
                    case TagDataType.Undefined:
                        return propertyItem.Value.Length == 4;
                    default:
                        return false;
                }
            }
            else
            {
                switch (propertyItem.Id)
                {
                    case 4096: // Interoperability IFD - RelatedImageFileFormat
                    case 4097: // Interoperability IFD - RelatedImageWidth
                    case 4098: // Interoperability IFD - RelatedImageHeight
                        return true;
                    default:
                        return false;
                }
            }
        }

        private static class GusssTagSectionHelper
        {
            internal static readonly Dictionary<int, MetadataKey[]> tiffAndExifTags = CreateTiffAndExifTagDictionary();

            private static Dictionary<int, MetadataKey[]> CreateTiffAndExifTagDictionary()
            {
                return MetadataKeyTable.Rows
                    .Where(i => i.Section == MetadataSection.Image || i.Section == MetadataSection.Exif)
                    .GroupBy(g => g.TagId)
                    .Select(s => new KeyValuePair<int, MetadataKey[]>(s.Key, s.ToArray()))
                    .ToDictionary(k => k.Key, k => k.Value);
            }
        }
    }
}
