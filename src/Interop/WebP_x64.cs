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
    internal static unsafe class WebP_x64
    {
        [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetImageInfo")]
        public static extern VP8StatusCode WebPGetImageInfo(byte* data, UIntPtr dataSize, out ImageInfo info);

        [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
        public static extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, UIntPtr outSize, int outStride);

        [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
        public static extern WebPEncodingError WebPSave(
            WebPWriteImage writeImageCallback,
            IntPtr scan0,
            int width,
            int height,
            int stride,
            EncoderOptions options,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MetadataCustomMarshaler))]
            EncoderMetadata metadata,
            WebPReportProgress callback);

        [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetadataSize")]
        public static extern uint GetMetadataSize(byte* iData, UIntPtr iDataSize, MetadataType type);

        [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetadata")]
        public static extern void ExtractMetadata(byte* iData, UIntPtr iDataSize, byte* metadataBytes, uint metadataSize, MetadataType type);
    }
}
