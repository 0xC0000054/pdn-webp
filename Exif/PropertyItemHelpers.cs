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

using System.Drawing;
using System.Drawing.Imaging;

namespace WebPFileType.Exif
{
    internal static class PropertyItemHelpers
    {
        internal static RotateFlipType GetOrientationTransform(PropertyItem propertyItem)
        {
            RotateFlipType transform = RotateFlipType.RotateNoneFlipNone;

            ushort exifValue = PaintDotNet.Exif.DecodeShortValue(propertyItem);

            if (exifValue >= 1 && exifValue <= 8)
            {
                switch (exifValue)
                {
                    case 1:
                        // Do nothing
                        transform = RotateFlipType.RotateNoneFlipNone;
                        break;
                    case 2:
                        // Flip horizontally.
                        transform = RotateFlipType.RotateNoneFlipX;
                        break;
                    case 3:
                        // Rotate 180 degrees.
                        transform = RotateFlipType.Rotate180FlipNone;
                        break;
                    case 4:
                        // Flip vertically.
                        transform = RotateFlipType.RotateNoneFlipY;
                        break;
                    case 5:
                        // Rotate 90 degrees clockwise and flip horizontally.
                        transform = RotateFlipType.Rotate90FlipX;
                        break;
                    case 6:
                        // Rotate 90 degrees clockwise.
                        transform = RotateFlipType.Rotate90FlipNone;
                        break;
                    case 7:
                        // Rotate 270 degrees clockwise and flip horizontally.
                        transform = RotateFlipType.Rotate270FlipX;
                        break;
                    case 8:
                        // Rotate 270 degrees clockwise.
                        transform = RotateFlipType.Rotate270FlipNone;
                        break;
                }
            }

            return transform;
        }

        internal static bool TryDecodeRational(PropertyItem propertyItem, out double value)
        {
            uint numerator;
            uint denominator;
            try
            {
                PaintDotNet.Exif.DecodeRationalValue(propertyItem, out numerator, out denominator);
            }
            catch
            {
                value = 0.0;
                return false;
            }

            if (denominator == 0)
            {
                // Avoid division by zero.
                value = 0.0;
                return false;
            }

            value = (double)numerator / denominator;
            return true;
        }

        internal static bool TryDecodeShort(PropertyItem propertyItem, out ushort value)
        {
            bool result;

            try
            {
                value = PaintDotNet.Exif.DecodeShortValue(propertyItem);
                result = true;
            }
            catch
            {
                value = 0;
                result = false;
            }

            return result;
        }
    }
}
