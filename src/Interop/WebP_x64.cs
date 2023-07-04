////////////////////////////////////////////////////////////////////////
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

using System;
using System.Runtime.InteropServices;

namespace WebPFileType.Interop
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    internal static unsafe partial class WebP_x64
    {
        [LibraryImport("WebP_x64.dll", EntryPoint = "WebPGetImageInfo")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial VP8StatusCode WebPGetImageInfo(byte* data, UIntPtr dataSize, out ImageInfo info);

        [LibraryImport("WebP_x64.dll", EntryPoint = "WebPGetImageMetadata")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool WebPGetImageMetadata(byte* data, UIntPtr dataSize, WebPSetDecoderMetadata callback);

        [LibraryImport("WebP_x64.dll", EntryPoint = "WebPLoad")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, UIntPtr outSize, int outStride);

        [LibraryImport("WebP_x64.dll", EntryPoint = "WebPSave")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
        public static partial WebPEncodingError WebPSave(
            WebPWriteImage writeImageCallback,
            IntPtr scan0,
            int width,
            int height,
            int stride,
            EncoderOptions options,
            EncoderMetadata metadata,
            WebPReportProgress callback);
    }
}
