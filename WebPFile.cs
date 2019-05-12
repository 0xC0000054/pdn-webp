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

using PaintDotNet;
using System;
using System.Runtime.InteropServices;
using WebPFileType.Properties;

namespace WebPFileType
{
    static class WebPFile
    {
        internal enum VP8StatusCode : int
        {
            Ok = 0,
            OutOfMemory,
            InvalidParam,
            BitStreamError,
            UnsupportedFeature,
            Suspended,
            UserAbort,
            NotEnoughData,
        }

        internal enum MetadataType : int
        {
            ColorProfile = 0,
            EXIF,
            XMP
        }

        internal enum WebPEncodingError : int
        {
            MetaDataEncoding = -2,
            ApiVersionMismatch = -1,
            Ok = 0,
            OutOfMemory = 1,           // memory error allocating objects
            BitStreamOutOfMemory = 2, // memory error while flushing bits
            NullParameter = 3,         // a pointer parameter is NULL
            InvalidConfiguration = 4,   // configuration is invalid
            BadDimension = 5,           // picture has invalid width/height
            PartitionZeroOverflow = 6,     // partition is bigger than 512k
            PartitionOverflow = 7,      // partition is bigger than 16M
            BadWrite = 8,               // error while flushing bytes
            FileTooBig = 9,            // file is bigger than 4G
            UserAbort = 10              // abort request by user
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void WebPReportProgress(int progress);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate void WebPWriteImage(IntPtr image, UIntPtr imageSize);

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class EncodeParams
        {
            [MarshalAs(UnmanagedType.R4)]
            public float quality;
            [MarshalAs(UnmanagedType.I4)]
            public WebPPreset preset;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class MetadataParams
        {
            public byte[] iccProfile;
            public uint iccProfileSize;
            public byte[] exif;
            public uint exifSize;
            public byte[] xmp;
            public uint xmpSize;

            public MetadataParams(byte[] iccProfileBytes, byte[] exifBytes, byte[] xmpBytes)
            {
                if (iccProfileBytes != null)
                {
                    iccProfile = (byte[])iccProfileBytes.Clone();
                    iccProfileSize = (uint)iccProfileBytes.Length;
                }

                if (exifBytes != null)
                {
                    exif = (byte[])exifBytes.Clone();
                    exifSize = (uint)exifBytes.Length;
                }

                if (xmpBytes != null)
                {
                    xmp = (byte[])xmpBytes.Clone();
                    xmpSize = (uint)xmpBytes.Length;
                }
            }
        }

        private const int WebPMaxDimension = 16383;
        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_32
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
            public static extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, UIntPtr outSize, int outStride);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
            public static extern WebPEncodingError WebPSave(
                WebPWriteImage writeImageCallback,
                IntPtr scan0,
                int width,
                int height,
                int stride,
                EncodeParams parameters,
                MetadataParams metaData,
                WebPReportProgress callback);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetadataSize")]
            public static extern uint GetMetadataSize(byte* iData, UIntPtr iDataSize, MetadataType type);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetadata")]
            public static extern void ExtractMetadata(byte* iData, UIntPtr iDataSize, byte* metadataBytes, uint metadataSize, MetadataType type);
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_64
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
            public static extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, UIntPtr outSize, int outStride);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
            public static extern WebPEncodingError WebPSave(
                WebPWriteImage writeImageCallback,
                IntPtr scan0,
                int width,
                int height,
                int stride,
                EncodeParams parameters,
                MetadataParams metaData,
                WebPReportProgress callback);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetadataSize")]
            public static extern uint GetMetadataSize(byte* iData, UIntPtr iDataSize, MetadataType type);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetadata")]
            public static extern void ExtractMetadata(byte* iData, UIntPtr iDataSize, byte* metadataBytes, uint metadataSize, MetadataType type);
        }

        /// <summary>
        /// Gets the dimension of the WebP image.
        /// </summary>
        /// <param name="data">The input image data.</param>
        /// <param name="width">The output width of the image.</param>
        /// <param name="height">The output height of the image.</param>
        /// <returns>true on success, otherwise false.</returns>
        internal static unsafe bool WebPGetDimensions(byte[] data, out int width, out int height)
        {
            fixed (byte* ptr = data)
            {
                if (IntPtr.Size == 8)
                {
                    return WebP_64.WebPGetDimensions(ptr, new UIntPtr((ulong)data.Length), out width, out height);
                }
                else
                {
                    return WebP_32.WebPGetDimensions(ptr, new UIntPtr((ulong)data.Length), out width, out height);
                }
            }
        }

        /// <summary>
        /// The WebP load function.
        /// </summary>
        /// <param name="data">The input image data</param>
        /// <param name="output">The output BitmapData.</param>
        /// <returns>VP8StatusCode.Ok on success.</returns>
        internal static unsafe VP8StatusCode WebPLoad(byte[] data, System.Drawing.Imaging.BitmapData output)
        {
            fixed (byte* ptr = data)
            {
                int stride = output.Stride;
                ulong outputSize = (ulong)stride * (ulong)output.Height;
                if (IntPtr.Size == 8)
                {
                    return WebP_64.WebPLoad(ptr, new UIntPtr((ulong)data.Length), (byte*)output.Scan0, new UIntPtr(outputSize), stride);
                }
                else
                {
                    return WebP_32.WebPLoad(ptr, new UIntPtr((ulong)data.Length), (byte*)output.Scan0, new UIntPtr(outputSize), stride);
                }
            }
        }

        /// <summary>
        /// The WebP save function.
        /// </summary>
        /// <param name="outputAllocator">The allocator for the managed output buffer.</param>
        /// <param name="input">The input surface.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// A pointer to the pinned managed array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputAllocator"/> is null.
        /// or
        /// <paramref name="input"/> is null.
        /// </exception>
        /// <exception cref="FormatException">The image exceeds 16383 pixels in width and/or height.</exception>
        internal static void WebPSave(
            WebPWriteImage writeImageCallback,
            Surface input,
            EncodeParams parameters,
            MetadataParams metaData,
            WebPReportProgress callback)
        {
            if (writeImageCallback == null)
            {
                throw new ArgumentNullException(nameof(writeImageCallback));
            }

            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Width > WebPMaxDimension || input.Height > WebPMaxDimension)
            {
                throw new FormatException(Resources.InvalidImageDimensions);
            }

            WebPEncodingError retVal = WebPEncodingError.Ok;

            if (IntPtr.Size == 8)
            {
                retVal = WebP_64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metaData, callback);
            }
            else
            {
                retVal = WebP_32.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metaData, callback);
            }
            GC.KeepAlive(writeImageCallback);

            if (retVal != WebPEncodingError.Ok)
            {
                switch (retVal)
                {
                    case WebPEncodingError.OutOfMemory:
                    case WebPEncodingError.BitStreamOutOfMemory:
                        throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
                    case WebPEncodingError.NullParameter:
                    case WebPEncodingError.InvalidConfiguration:
                    case WebPEncodingError.PartitionZeroOverflow:
                    case WebPEncodingError.PartitionOverflow:
                    case WebPEncodingError.BadWrite:
                        throw new WebPException(Resources.EncoderGenericError);
                    case WebPEncodingError.FileTooBig:
                        throw new WebPException(Resources.EncoderFileTooBig);

                    case WebPEncodingError.ApiVersionMismatch:
                        throw new WebPException(Resources.ApiVersionMismatch);
                    case WebPEncodingError.MetaDataEncoding:
                        throw new WebPException(Resources.EncoderMetaDataError);
                }
            }
        }

        internal static unsafe uint GetMetadataSize(byte[] data, MetadataType type)
        {
            uint metadataSize;

            fixed (byte* ptr = data)
            {
                if (IntPtr.Size == 8)
                {
                    metadataSize = WebP_64.GetMetadataSize(ptr, new UIntPtr((ulong)data.Length), type);
                }
                else
                {
                    metadataSize = WebP_32.GetMetadataSize(ptr, new UIntPtr((ulong)data.Length), type);
                }
            }

            return metadataSize;
        }

        internal static unsafe void ExtractMetadata(byte[] data, MetadataType type, byte[] outData, uint outSize)
        {
            fixed (byte* ptr = data, outPtr = outData)
            {
                if (IntPtr.Size == 8)
                {
                    WebP_64.ExtractMetadata(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
                else
                {
                    WebP_32.ExtractMetadata(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
            }
        }
    }
}
