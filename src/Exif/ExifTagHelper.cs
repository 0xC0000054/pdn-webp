////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2023 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace WebPFileType.Exif
{
    internal static class ExifTagHelper
    {
        private static readonly HashSet<ushort> supportedTiffImageTagsForWriting = new()
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
    }
}
