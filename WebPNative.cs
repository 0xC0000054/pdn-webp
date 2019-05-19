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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using WebPFileType.Properties;

namespace WebPFileType
{
    internal static class WebPNative
    {
        internal enum MetadataType : int
        {
            ColorProfile = 0,
            EXIF,
            XMP
        }

        private enum VP8StatusCode : int
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

        private enum WebPEncodingError : int
        {
            MetadataEncoding = -2,
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
        [return: MarshalAs(UnmanagedType.U1)]
        internal delegate bool WebPReportProgress(int progress);

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
            public byte[] exif;
            public byte[] xmp;

            public MetadataParams(byte[] iccProfileBytes, byte[] exifBytes, byte[] xmpBytes)
            {
                if (iccProfileBytes != null)
                {
                    iccProfile = (byte[])iccProfileBytes.Clone();
                }

                if (exifBytes != null)
                {
                    exif = (byte[])exifBytes.Clone();
                }

                if (xmpBytes != null)
                {
                    xmp = (byte[])xmpBytes.Clone();
                }
            }
        }

        internal const int WebPMaxDimension = 16383;

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
                [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MetadataCustomMarshaler))]
                MetadataParams metadata,
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
                [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MetadataCustomMarshaler))]
                MetadataParams metadata,
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
        /// <param name="webpBytes">The input image data</param>
        /// <exception cref="OutOfMemoryException">Insufficient memory to load the WebP image.</exception>
        /// <exception cref="WebPException">
        /// The WebP image is invalid.
        /// -or-
        /// A native API parameter is invalid.
        /// </exception>
        internal static unsafe void WebPLoad(byte[] webpBytes, System.Drawing.Imaging.BitmapData output)
        {
            VP8StatusCode status;

            fixed (byte* ptr = webpBytes)
            {
                int stride = output.Stride;
                ulong outputSize = (ulong)stride * (ulong)output.Height;
                if (IntPtr.Size == 8)
                {
                    status = WebP_64.WebPLoad(ptr, new UIntPtr((ulong)webpBytes.Length), (byte*)output.Scan0, new UIntPtr(outputSize), stride);
                }
                else
                {
                    status = WebP_32.WebPLoad(ptr, new UIntPtr((ulong)webpBytes.Length), (byte*)output.Scan0, new UIntPtr(outputSize), stride);
                }
            }

            if (status != VP8StatusCode.Ok)
            {
                switch (status)
                {
                    case VP8StatusCode.OutOfMemory:
                        throw new OutOfMemoryException();
                    case VP8StatusCode.InvalidParam:
                        throw new WebPException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidParameterFormat, nameof(WebPLoad)));
                    case VP8StatusCode.BitStreamError:
                    case VP8StatusCode.UnsupportedFeature:
                    case VP8StatusCode.NotEnoughData:
                    default:
                        throw new WebPException(Resources.InvalidWebPImage);
                }
            }
        }

        /// <summary>
        /// The WebP save function.
        /// </summary>
        /// <param name="writeImageCallback">The callback used to write the WebP image.</param>
        /// <param name="input">The input surface.</param>
        /// <param name="parameters">The encode parameters.</param>
        /// <param name="metadata">The image metadata.</param>
        /// <param name="callback">The progress callback.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writeImageCallback"/> is null.
        /// or
        /// <paramref name="input"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory to save the image.</exception>
        /// <exception cref="WebPException">The encoder returned a non-memory related error.</exception>
        internal static void WebPSave(
            WebPWriteImage writeImageCallback,
            Surface input,
            EncodeParams parameters,
            MetadataParams metadata,
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

            WebPEncodingError retVal = WebPEncodingError.Ok;

            if (IntPtr.Size == 8)
            {
                retVal = WebP_64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metadata, callback);
            }
            else
            {
                retVal = WebP_32.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metadata, callback);
            }
            GC.KeepAlive(writeImageCallback);

            if (retVal != WebPEncodingError.Ok)
            {
                switch (retVal)
                {
                    case WebPEncodingError.OutOfMemory:
                    case WebPEncodingError.BitStreamOutOfMemory:
                        throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
                    case WebPEncodingError.FileTooBig:
                        throw new WebPException(Resources.EncoderFileTooBig);
                    case WebPEncodingError.ApiVersionMismatch:
                        throw new WebPException(Resources.ApiVersionMismatch);
                    case WebPEncodingError.MetadataEncoding:
                        throw new WebPException(Resources.EncoderMetadataError);
                    case WebPEncodingError.UserAbort:
                        throw new OperationCanceledException();
                    case WebPEncodingError.BadDimension:
                        throw new WebPException(Resources.InvalidImageDimensions);
                    case WebPEncodingError.NullParameter:
                        throw new WebPException(Resources.EncoderNullParameter);
                    case WebPEncodingError.InvalidConfiguration:
                        throw new WebPException(Resources.EncoderInvalidConfiguration);
                    case WebPEncodingError.PartitionZeroOverflow:
                        throw new WebPException(Resources.EncoderPartitionZeroOverflow);
                    case WebPEncodingError.PartitionOverflow:
                        throw new WebPException(Resources.EncoderPartitionOverflow);
                    case WebPEncodingError.BadWrite:
                        throw new IOException(Resources.EncoderBadWrite);
                    default:
                        throw new WebPException(Resources.EncoderGenericError);
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
