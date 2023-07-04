﻿////////////////////////////////////////////////////////////////////////
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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WebPFileType.Interop
{
    [NativeMarshalling(typeof(Marshaller))]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly partial struct ImageInfo
    {
        public readonly int width;
        public readonly int height;
        public readonly bool hasAnimation;

        public ImageInfo(int width, int height, bool hasAnimation)
        {
            this.width = width;
            this.height = height;
            this.hasAnimation = hasAnimation;
        }
    }
}
