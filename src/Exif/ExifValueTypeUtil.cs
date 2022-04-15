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

using PaintDotNet.Imaging;

namespace WebPFileType.Exif
{
    internal static class ExifValueTypeUtil
    {
        /// <summary>
        /// Gets the size in bytes of a <see cref="TagDataType"/> value.
        /// </summary>
        /// <param name="type">The tag type.</param>
        /// <returns>
        /// The size of the value in bytes.
        /// </returns>
        public static int GetSizeInBytes(ExifValueType type)
        {
            switch (type)
            {
                case ExifValueType.Byte:
                case ExifValueType.Ascii:
                case ExifValueType.Undefined:
                case (ExifValueType)6: // SByte
                    return 1;
                case ExifValueType.Short:
                case ExifValueType.SShort:
                    return 2;
                case ExifValueType.Long:
                case ExifValueType.SLong:
                case ExifValueType.Float:
                case (ExifValueType)13: // IFD
                    return 4;
                case ExifValueType.Rational:
                case ExifValueType.SRational:
                case ExifValueType.Double:
                    return 8;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Determines whether the values fit in the offset field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// <see langword="true"/> if the values fit in the offset field; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ValueFitsInOffsetField(ExifValueType type, uint count)
        {
            switch (type)
            {
                case ExifValueType.Byte:
                case ExifValueType.Ascii:
                case ExifValueType.Undefined:
                case (ExifValueType)6: // SByte
                    return count <= 4;
                case ExifValueType.Short:
                case ExifValueType.SShort:
                    return count <= 2;
                case ExifValueType.Long:
                case ExifValueType.SLong:
                case ExifValueType.Float:
                case (ExifValueType)13: // IFD
                    return count <= 1;
                case ExifValueType.Rational:
                case ExifValueType.SRational:
                case ExifValueType.Double:
                default:
                    return false;
            }
        }
    }
}
