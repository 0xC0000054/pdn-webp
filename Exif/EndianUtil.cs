////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2020 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

namespace WebPFileType.Exif
{
    internal static class EndianUtil
    {
        public static ushort Swap(ushort value)
        {
            return (ushort)(((value & 0xff00) >> 8) | ((value & 0x00ff) << 8));
        }

        public static uint Swap(uint value)
        {
           return ((value & 0xff000000) >> 24) |
                  ((value & 0x00ff0000) >> 8 ) |
                  ((value & 0x0000ff00) << 8 ) |
                  ((value & 0x000000ff) << 24);
        }

        public static ulong Swap(ulong value)
        {
            return ((value & 0xff00000000000000) >> 56) |
                   ((value & 0x00ff000000000000) >> 40) |
                   ((value & 0x0000ff0000000000) >> 24) |
                   ((value & 0x000000ff00000000) >> 8 ) |
                   ((value & 0x00000000ff000000) << 8 ) |
                   ((value & 0x0000000000ff0000) << 24) |
                   ((value & 0x000000000000ff00) << 40) |
                   ((value & 0x00000000000000ff) << 56);
        }
    }
}
