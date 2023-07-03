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
        private delegate WebPEncodingError WebPWriteImage(IntPtr image, UIntPtr imageSize);

        [StructLayout(LayoutKind.Sequential)]
        internal sealed class EncodeParams
        {
            [MarshalAs(UnmanagedType.R4)]
            public float quality;
            [MarshalAs(UnmanagedType.I4)]
            public WebPPreset preset;
            [MarshalAs(UnmanagedType.U1)]
            public bool lossless;
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct ImageInfo
        {
            public int width;
            public int height;
            [MarshalAs(UnmanagedType.U1)]
            public bool hasAnimation;
        }

        internal const int WebPMaxDimension = 16383;

        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_x64
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetImageInfo")]
            public static extern VP8StatusCode WebPGetImageInfo(byte* data, UIntPtr dataSize, out ImageInfo info);

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

        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_ARM64
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_ARM64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetImageInfo")]
            public static extern VP8StatusCode WebPGetImageInfo(byte* data, UIntPtr dataSize, out ImageInfo info);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_ARM64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
            public static extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, UIntPtr outSize, int outStride);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_ARM64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
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
            [DllImport("WebP_ARM64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetadataSize")]
            public static extern uint GetMetadataSize(byte* iData, UIntPtr iDataSize, MetadataType type);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
            [DllImport("WebP_ARM64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetadata")]
            public static extern void ExtractMetadata(byte* iData, UIntPtr iDataSize, byte* metadataBytes, uint metadataSize, MetadataType type);
        }

        /// <summary>
        /// Gets the WebP image information.
        /// </summary>
        /// <param name="data">The input image data.</param>
        /// <param name="info">The output image information.</param>
        /// <exception cref="OutOfMemoryException">Insufficient memory to load the WebP image.</exception>
        /// <exception cref="WebPException">
        /// The WebP image is invalid.
        /// -or-
        /// A native API parameter is invalid.
        /// </exception>
        internal static unsafe void WebPGetImageInfo(byte[] data, out ImageInfo info)
        {
            VP8StatusCode status;

            fixed (byte* ptr = data)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    status = WebP_x64.WebPGetImageInfo(ptr, new UIntPtr((ulong)data.Length), out info);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    status = WebP_ARM64.WebPGetImageInfo(ptr, new UIntPtr((ulong)data.Length), out info);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }

            if (status != VP8StatusCode.Ok)
            {
                switch (status)
                {
                    case VP8StatusCode.OutOfMemory:
                        throw new OutOfMemoryException();
                    case VP8StatusCode.InvalidParam:
                        throw new WebPException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidParameterFormat, nameof(WebPGetImageInfo)));
                    case VP8StatusCode.UnsupportedFeature:
                        throw new WebPException(Resources.UnsupportedWebPFeature);
                    case VP8StatusCode.BitStreamError:
                    case VP8StatusCode.NotEnoughData:
                    default:
                        throw new WebPException(Resources.InvalidWebPImage);
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
        internal static unsafe void WebPLoad(byte[] webpBytes, Surface output)
        {
            VP8StatusCode status;

            fixed (byte* ptr = webpBytes)
            {
                int stride = output.Stride;
                ulong outputSize = (ulong)stride * (ulong)output.Height;

                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    status = WebP_x64.WebPLoad(ptr, new UIntPtr((ulong)webpBytes.Length), (byte*)output.Scan0.VoidStar, new UIntPtr(outputSize), stride);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    status = WebP_ARM64.WebPLoad(ptr, new UIntPtr((ulong)webpBytes.Length), (byte*)output.Scan0.VoidStar, new UIntPtr(outputSize), stride);
                }
                else
                {
                    throw new PlatformNotSupportedException();
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
                    case VP8StatusCode.UnsupportedFeature:
                        throw new WebPException(Resources.UnsupportedWebPFeature);
                    case VP8StatusCode.BitStreamError:
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
            Surface input,
            Stream output,
            EncodeParams parameters,
            MetadataParams metadata,
            WebPReportProgress callback)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StreamIOHandler handler = new(output);
            WebPWriteImage writeImageCallback = handler.WriteImageCallback;

            WebPEncodingError retVal = WebPEncodingError.Ok;

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                retVal = WebP_x64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metadata, callback);
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                retVal = WebP_ARM64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, parameters, metadata, callback);
            }
            else
            {
                throw new PlatformNotSupportedException();
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
                        if (handler.WriteException != null)
                        {
                            throw new IOException(Resources.EncoderBadWrite, handler.WriteException);
                        }
                        else
                        {
                            throw new IOException(Resources.EncoderBadWrite);
                        }
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
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    metadataSize = WebP_x64.GetMetadataSize(ptr, new UIntPtr((ulong)data.Length), type);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    metadataSize = WebP_ARM64.GetMetadataSize(ptr, new UIntPtr((ulong)data.Length), type);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }

            return metadataSize;
        }

        internal static unsafe void ExtractMetadata(byte[] data, MetadataType type, byte[] outData, uint outSize)
        {
            fixed (byte* ptr = data, outPtr = outData)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    WebP_x64.ExtractMetadata(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    WebP_ARM64.ExtractMetadata(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }
        }

        private sealed class StreamIOHandler
        {
            private readonly Stream output;

            public StreamIOHandler(Stream output)
            {
                this.output = output ?? throw new ArgumentNullException(nameof(output));
            }

            public Exception WriteException { get; private set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage(
                "Microsoft.Design",
                "CA1031:DoNotCatchGeneralExceptionTypes",
                Justification = "The exception will be re-thrown after WebPSave returns the error code.")]
            public WebPEncodingError WriteImageCallback(IntPtr image, UIntPtr imageSize)
            {
                if (image == IntPtr.Zero)
                {
                    return WebPEncodingError.NullParameter;
                }

                if (imageSize == UIntPtr.Zero)
                {
                    // Ignore zero-length images.
                    return WebPEncodingError.Ok;
                }

                // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
                const int MaxBufferSize = 81920;

                try
                {
                    long size = checked((long)imageSize.ToUInt64());

                    int bufferSize = (int)Math.Min(size, MaxBufferSize);

                    byte[] streamBuffer = new byte[bufferSize];

                    output.SetLength(size);

                    long offset = 0;
                    long remaining = size;

                    while (remaining > 0)
                    {
                        int copySize = (int)Math.Min(MaxBufferSize, remaining);

                        Marshal.Copy(new IntPtr(image.ToInt64() + offset), streamBuffer, 0, copySize);

                        output.Write(streamBuffer, 0, copySize);

                        offset += copySize;
                        remaining -= copySize;
                    }
                }
                catch (OperationCanceledException)
                {
                    return WebPEncodingError.UserAbort;
                }
                catch (Exception ex)
                {
                    WriteException = ex;
                    return WebPEncodingError.BadWrite;
                }

                return WebPEncodingError.Ok;
            }
        }
    }
}
