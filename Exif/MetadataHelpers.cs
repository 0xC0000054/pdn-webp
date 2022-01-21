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

namespace WebPFileType.Exif
{
    internal static class MetadataHelpers
    {
        internal static void ApplyOrientationTransform(MetadataEntry entry, ref Surface surface)
        {
            if (TryDecodeShort(entry, out ushort exifValue))
            {
                if (exifValue >= TiffConstants.Orientation.TopLeft && exifValue <= TiffConstants.Orientation.LeftBottom)
                {
                    switch (exifValue)
                    {
                        case TiffConstants.Orientation.TopLeft:
                            // Do nothing
                            break;
                        case TiffConstants.Orientation.TopRight:
                            // Flip horizontally.
                            ImageTransform.FlipHorizontal(surface);
                            break;
                        case TiffConstants.Orientation.BottomRight:
                            // Rotate 180 degrees.
                            ImageTransform.Rotate180(surface);
                            break;
                        case TiffConstants.Orientation.BottomLeft:
                            // Flip vertically.
                            ImageTransform.FlipVertical(surface);
                            break;
                        case TiffConstants.Orientation.LeftTop:
                            // Rotate 90 degrees clockwise and flip horizontally.
                            ImageTransform.Rotate90CW(ref surface);
                            ImageTransform.FlipHorizontal(surface);
                            break;
                        case TiffConstants.Orientation.RightTop:
                            // Rotate 90 degrees clockwise.
                            ImageTransform.Rotate90CW(ref surface);
                            break;
                        case TiffConstants.Orientation.RightBottom:
                            // Rotate 270 degrees clockwise and flip horizontally.
                            ImageTransform.Rotate270CW(ref surface);
                            ImageTransform.FlipHorizontal(surface);
                            break;
                        case TiffConstants.Orientation.LeftBottom:
                            // Rotate 270 degrees clockwise.
                            ImageTransform.Rotate270CW(ref surface);
                            break;
                    }
                }
            }
        }

        internal static byte[] EncodeLong(uint value)
        {
            return new byte[]
            {
                (byte)(value & 0xff),
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            };
        }

        internal static byte[] EncodeShort(ushort value)
        {
            return new byte[]
            {
                (byte)(value & 0xff),
                (byte)(value >> 8)
            };
        }

        internal static bool TryDecodeRational(MetadataEntry entry, out double value)
        {
            uint numerator;
            uint denominator;

            if (entry.Type != TagDataType.Rational || entry.LengthInBytes != 8)
            {
                value = 0.0;
                return false;
            }

            byte[] data = entry.GetDataReadOnly();

            numerator = (uint)(data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
            denominator = (uint)(data[4] | (data[5] << 8) | (data[6] << 16) | (data[7] << 24));

            if (denominator == 0)
            {
                // Avoid division by zero.
                value = 0.0;
                return false;
            }

            value = (double)numerator / denominator;
            return true;
        }

        internal static bool TryDecodeShort(MetadataEntry entry, out ushort value)
        {
            if (entry.Type != TagDataType.Short || entry.LengthInBytes != 2)
            {
                value = 0;
                return false;
            }

            byte[] data = entry.GetDataReadOnly();

            value = (ushort)(data[0] | (data[1] << 8));

            return true;
        }
    }
}
