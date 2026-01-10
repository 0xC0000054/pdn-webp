////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace WebPFileType.Interop
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static unsafe partial class WebP_ARM64
    {
        [LibraryImport("WebP_ARM64.dll", EntryPoint = "GetLibWebPVersion")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial int GetLibWebPVersion();

        [LibraryImport("WebP_ARM64.dll", EntryPoint = "WebPLoad")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial WebPStatus WebPLoad(byte* data,
                                                  UIntPtr dataSize,
                                                  WebPCreateImage createImage,
                                                  WebPSetDecoderMetadata setDecoderMetadata);

        [LibraryImport("WebP_ARM64.dll", EntryPoint = "WebPSave")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial WebPStatus WebPSave(WebPWriteImage writeImageCallback,
                                                  IntPtr scan0,
                                                  int width,
                                                  int height,
                                                  int stride,
                                                  in EncoderOptions options,
                                                  in EncoderMetadata? metadata,
                                                  WebPReportProgress? callback);
    }
}
