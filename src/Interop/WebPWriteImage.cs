////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2024 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace WebPFileType.Interop
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate WebPStatus WebPWriteImage(IntPtr image, UIntPtr imageSize);
}
