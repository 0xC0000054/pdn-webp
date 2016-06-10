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

        internal enum MetaDataType : int
        {
            ColorProfile = 0,
            EXIF,
            XMP
        }

        internal enum WebPMuxError : int
        {
            Ok = 1,
            NotFound = 0,
            InvalidArgument = -1,
            BadData = -2,
            MemoryError = -3,
            NotEnoughData = -4
        }

        internal enum WebPEncodingError : int
        {
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct EncodeParams
        {
            [MarshalAs(UnmanagedType.R4)]
            public float quality;
            [MarshalAs(UnmanagedType.I4)]
            public WebPPreset preset;
            public int method;
            public int noiseShaping;
            [MarshalAs(UnmanagedType.I4)]
            public WebPFilterType filterType;
            public int filterStrength;
            public int sharpness;
            public int fileSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MetaDataParams
        {
            public byte[] iccProfile;
            public uint iccProfileSize;
            public byte[] exif;
            public uint exifSize;
            public byte[] xmp;
            public uint xmpSize;
        }

        private const int WebPMaxDimension = 16383;
        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_32
        {
            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
            public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, int outSize, int outStride);

            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
            public static extern WebPEncodingError WebPSave(out IntPtr output, PinnedByteArrayAllocDelegate outputAllocator, IntPtr iBitmap, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetaDataSize")]
            public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

            [DllImport("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetaData")]
            public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);

            [DllImportAttribute("WebP_x86.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetMetaData")]
            public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, PinnedByteArrayAllocDelegate outputAllocator, MetaDataParams metaData);
        }

        [System.Security.SuppressUnmanagedCodeSecurity]
        private unsafe static class WebP_64
        {
            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPGetDimensions")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static unsafe extern bool WebPGetDimensions(byte* data, UIntPtr dataSize, out int width, out int height);

            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPLoad")]
            public static unsafe extern VP8StatusCode WebPLoad(byte* data, UIntPtr dataSize, byte* outData, int outSize, int outStride);

            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "WebPSave")]
            public static unsafe extern WebPEncodingError WebPSave(out IntPtr output, PinnedByteArrayAllocDelegate outputAllocator, IntPtr scan0, int iWidth, int iHeight, int iStride, EncodeParams parameters, WebPReportProgress callback);

            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMetaDataSize")]
            public static unsafe extern void GetMetaDataSize(byte* iData, UIntPtr iDataSize, MetaDataType type, out uint metaDataSize);

            [DllImport("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "ExtractMetaData")]
            public static unsafe extern void ExtractMetaData(byte* iData, UIntPtr iDataSize, byte* metaDataBytes, uint metaDataSize, MetaDataType type);

            [DllImportAttribute("WebP_x64.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "SetMetaData")]
            public static unsafe extern WebPMuxError SetMetaData(byte* image, UIntPtr imageSize, ref IntPtr outImage, PinnedByteArrayAllocDelegate outputAllocator, MetaDataParams metaData);
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
        /// <param name="width">The width 0f the resulting image.</param>
        /// <param name="outputStride">The height of the resulting image.</param>
        /// <param name="outPtr">The output byte array.</param>
        /// <returns>VP8StatusCode.Ok on success.</returns>
        internal static unsafe VP8StatusCode WebPLoad(byte[] data, byte* outPtr, int outputSize, int outputStride)
        {
            fixed (byte* ptr = data)
            {
                if (IntPtr.Size == 8)
                {
                    return WebP_64.WebPLoad(ptr, new UIntPtr((ulong)data.Length), outPtr, outputSize, outputStride);
                }
                else
                {
                    return WebP_32.WebPLoad(ptr, new UIntPtr((ulong)data.Length), outPtr, outputSize, outputStride);
                }
            }
        }

        /// <summary>
        /// The WebP save function.
        /// </summary>
        /// <param name="outputAllocator">The allocator for the managed output buffer.</param>
        /// <param name="scan0">The input bitmap.</param>
        /// <param name="width">Width of the input bitmap.</param>
        /// <param name="height">Height of the input bitmap.</param>
        /// <param name="stride">The stride of the input bitmap.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>
        /// A pointer to the pinned managed array.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="outputAllocator"/> is null.</exception>
        /// <exception cref="FormatException">The image exceeds 16383 pixels in width and/or height.</exception>
        internal static IntPtr WebPSave(
            PinnedByteArrayAllocator outputAllocator,
            IntPtr scan0,
            int width,
            int height,
            long stride,
            EncodeParams parameters,
            WebPReportProgress callback)
        {
            if (outputAllocator == null)
            {
                throw new ArgumentNullException(nameof(outputAllocator));
            }

            if (width > WebPMaxDimension || height > WebPMaxDimension)
            {
                throw new FormatException(Resources.InvalidImageDimensions);
            }


            WebPEncodingError retVal = WebPEncodingError.Ok;
            IntPtr outPtr = IntPtr.Zero;

            PinnedByteArrayAllocDelegate allocateFn = new PinnedByteArrayAllocDelegate(outputAllocator.AllocateArray);
            if (IntPtr.Size == 8)
            {
                retVal = WebP_64.WebPSave(out outPtr, allocateFn, scan0, width, height, (int)stride, parameters, callback);
            }
            else
            {
                retVal = WebP_32.WebPSave(out outPtr, allocateFn, scan0, width, height, (int)stride, parameters, callback);
            }
            GC.KeepAlive(allocateFn);

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
                }
            }

            return outPtr;
        }

        internal static unsafe uint GetMetaDataSize(byte[] data, MetaDataType type)
        {
            uint metaDataSize = 0U;

            fixed (byte* ptr = data)
            {
                if (IntPtr.Size == 8)
                {
                    WebP_64.GetMetaDataSize(ptr, new UIntPtr((ulong)data.Length), type, out metaDataSize);
                }
                else
                {
                    WebP_32.GetMetaDataSize(ptr, new UIntPtr((ulong)data.Length), type, out metaDataSize);
                }
            }

            return metaDataSize;
        }

        internal static unsafe void ExtractMetadata(byte[] data, MetaDataType type, byte[] outData, uint outSize)
        {
            fixed (byte* ptr = data, outPtr = outData)
            {
                if (IntPtr.Size == 8)
                {
                    WebP_64.ExtractMetaData(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
                else
                {
                    WebP_32.ExtractMetaData(ptr, new UIntPtr((ulong)data.Length), outPtr, outSize, type);
                }
            }
        }

        /// <summary>
        /// Sets the meta data for the WebP image.
        /// </summary>
        /// <param name="imageBytes">The existing WebP image.</param>
        /// <param name="metaData">The meta data to embed into the image.</param>
        /// <param name="outputAllocator">The allocator for the managed output buffer.</param>
        /// <returns>
        /// A pointer to the pinned managed array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageBytes"/> is null.
        /// -or-
        /// <paramref name="outputAllocator"/> is null.
        /// </exception>
        internal static unsafe IntPtr SetMetaData(byte[] imageBytes, MetaDataParams metaData, PinnedByteArrayAllocator outputAllocator)
        {
            if (imageBytes == null)
            {
                throw new ArgumentNullException(nameof(imageBytes));
            }
            if (outputAllocator == null)
            {
                throw new ArgumentNullException(nameof(outputAllocator));
            }

            IntPtr outPtr = IntPtr.Zero;

            fixed (byte* ptr = imageBytes)
            {
                WebPMuxError error = WebPMuxError.Ok;

                PinnedByteArrayAllocDelegate allocateFn = new PinnedByteArrayAllocDelegate(outputAllocator.AllocateArray);
                if (IntPtr.Size == 8)
                {
                    error = WebP_64.SetMetaData(ptr, new UIntPtr((ulong)imageBytes.Length), ref outPtr, allocateFn, metaData);
                }
                else
                {
                    error = WebP_32.SetMetaData(ptr, new UIntPtr((ulong)imageBytes.Length), ref outPtr, allocateFn, metaData);
                }
                GC.KeepAlive(allocateFn);

                if (error != WebPMuxError.Ok)
                {
                    switch (error)
                    {
                        case WebPMuxError.MemoryError:
                            throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
                        case WebPMuxError.NotFound:
                        case WebPMuxError.InvalidArgument:
                        case WebPMuxError.BadData:
                        case WebPMuxError.NotEnoughData:
                            throw new WebPException(Resources.EncoderGenericError);
                        default:
                            break;
                    }
                }
            }

            return outPtr;
        }


    }
}
