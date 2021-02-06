////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2021 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

namespace WebPFileType.Exif
{
    internal static class TagDataTypeUtil
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
                case TagDataType.Undefined:
                case TagDataType.SLong:
                case TagDataType.SRational:
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

        /// <summary>
        /// Determines whether the values fit in the offset field.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// <see langword="true"/> if the values fit in the offset field; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool ValueFitsInOffsetField(TagDataType type, uint count)
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
                    return count <= 1;
                case TagDataType.Rational:
                case TagDataType.SRational:
                case TagDataType.Double:
                default:
                    return false;
            }
        }
    }
}
