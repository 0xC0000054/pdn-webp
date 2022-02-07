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
    internal static class ImageTransform
    {
        internal static unsafe void FlipHorizontal(Surface surface)
        {
            int lastColumn = surface.Width - 1;
            int flipWidth = surface.Width / 2;

            for (int y = 0; y < surface.Height; y++)
            {
                for (int x = 0; x < flipWidth; x++)
                {
                    int sampleColumn = lastColumn - x;

                    ColorBgra temp = surface[x, y];
                    surface[x, y] = surface[sampleColumn, y];
                    surface[sampleColumn, y] = temp;
                }
            }
        }

        internal static unsafe void FlipVertical(Surface surface)
        {
            int lastRow = surface.Height - 1;
            int flipHeight = surface.Height / 2;

            for (int x = 0; x < surface.Width; x++)
            {
                for (int y = 0; y < flipHeight; y++)
                {
                    int sampleRow = lastRow - y;

                    ColorBgra temp = surface[x, y];
                    surface[x, y] = surface[x, sampleRow];
                    surface[x, sampleRow] = temp;
                }
            }
        }

        internal static unsafe void Rotate90CW(ref Surface surface)
        {
            Surface temp = null;
            try
            {
                int newWidth = surface.Height;
                int newHeight = surface.Width;

                temp = new Surface(newWidth, newHeight);

                int lastColumn = newWidth - 1;

                for (int y = 0; y < newWidth; y++)
                {
                    ColorBgra* srcPtr = surface.GetRowPointerUnchecked(y);
                    ColorBgra* dstPtr = temp.GetPointPointerUnchecked(lastColumn - y, 0);

                    for (int x = 0; x < newHeight; x++)
                    {
                        *dstPtr = *srcPtr;

                        srcPtr++;
                        dstPtr += newWidth;
                    }
                }

                surface.Dispose();
                surface = temp;
                temp = null;
            }
            finally
            {
                temp?.Dispose();
            }
        }

        internal static unsafe void Rotate180(Surface surface)
        {
            int width = surface.Width;
            int height = surface.Height;

            int halfHeight = height / 2;
            int lastColumn = width - 1;

            for (int y = 0; y < halfHeight; y++)
            {
                ColorBgra* topPtr = surface.GetRowPointerUnchecked(y);
                ColorBgra* bottomPtr = surface.GetPointPointerUnchecked(lastColumn, height - y - 1);

                for (int x = 0; x < width; x++)
                {
                    ColorBgra temp = *bottomPtr;
                    *bottomPtr = *topPtr;
                    *topPtr = temp;

                    topPtr++;
                    bottomPtr--;
                }
            }

            // The middle row must be handled separately if the height is odd.
            if ((height & 1) == 1)
            {
                int halfWidth = width / 2;

                ColorBgra* leftPtr = surface.GetRowPointerUnchecked(halfHeight);
                ColorBgra* rightPtr = surface.GetPointPointerUnchecked(lastColumn, halfHeight);

                for (int x = 0; x < halfWidth; x++)
                {
                    ColorBgra temp = *rightPtr;
                    *rightPtr = *leftPtr;
                    *leftPtr = temp;

                    leftPtr++;
                    rightPtr--;
                }
            }
        }

        internal static unsafe void Rotate270CW(ref Surface surface)
        {
            // Rotating 270 degrees clockwise is equivalent to rotating 90 degrees counter-clockwise.

            Surface temp = null;
            try
            {
                int newWidth = surface.Height;
                int newHeight = surface.Width;

                temp = new Surface(newWidth, newHeight);

                int lastRow = newHeight - 1;

                for (int y = 0; y < newWidth; y++)
                {
                    ColorBgra* srcPtr = surface.GetPointPointerUnchecked(0, y);
                    ColorBgra* dstPtr = temp.GetPointPointerUnchecked(y, lastRow);

                    for (int x = 0; x < newHeight; x++)
                    {
                        *dstPtr = *srcPtr;

                        srcPtr++;
                        dstPtr -= newWidth;
                    }
                }

                surface.Dispose();
                surface = temp;
                temp = null;
            }
            finally
            {
                temp?.Dispose();
            }
        }
    }
}
