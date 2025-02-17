////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2025 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;

namespace WebPFileType.Exif
{
    internal static class TiffConstants
    {
        internal static ReadOnlySpan<byte> BigEndianByteOrderMarker => "MM"u8;
        internal static ReadOnlySpan<byte> LittleEndianByteOrderMarker => "II"u8;
        internal const ushort Signature = 42;

        internal static class Tags
        {
            internal const ushort StripOffsets = 273;
            internal const ushort RowsPerStrip = 278;
            internal const ushort StripByteCounts = 279;
            internal const ushort SubIFDs = 330;
            internal const ushort ThumbnailOffset = 513;
            internal const ushort ThumbnailLength = 514;
            internal const ushort ExifIFD = 34665;
            internal const ushort GpsIFD = 34853;
            internal const ushort InteropIFD = 40965;
        }

        internal static class Orientation
        {
            /// <summary>
            /// The 0th row is at the visual top of the image, and the 0th column is the visual left-hand side
            /// </summary>
            internal const ushort TopLeft = 1;

            /// <summary>
            /// The 0th row is at the visual top of the image, and the 0th column is the visual right-hand side.
            /// </summary>
            internal const ushort TopRight = 2;

            /// <summary>
            /// The 0th row represents the visual bottom of the image, and the 0th column represents the visual right-hand side.
            /// </summary>
            internal const ushort BottomRight = 3;

            /// <summary>
            /// The 0th row represents the visual bottom of the image, and the 0th column represents the visual left-hand side.
            /// </summary>
            internal const ushort BottomLeft = 4;

            /// <summary>
            /// The 0th row represents the visual left-hand side of the image, and the 0th column represents the visual top.
            /// </summary>
            internal const ushort LeftTop = 5;

            /// <summary>
            /// The 0th row represents the visual right-hand side of the image, and the 0th column represents the visual top.
            /// </summary>
            internal const ushort RightTop = 6;

            /// <summary>
            /// The 0th row represents the visual right-hand side of the image, and the 0th column represents the visual bottom.
            /// </summary>
            internal const ushort RightBottom = 7;

            /// <summary>
            /// The 0th row represents the visual left-hand side of the image, and the 0th column represents the visual bottom.
            /// </summary>
            internal const ushort LeftBottom = 8;
        }

        internal static class ResolutionUnit
        {
            internal const ushort Inch = 2;
            internal const ushort Centimeter = 3;
        }
    }
}
