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
using WebPFileType.Interop;
using WebPFileType.Properties;

namespace WebPFileType
{
    internal static class WebPNative
    {
        internal const int WebPMaxDimension = 16383;

        /// <summary>
        /// Gets the libwebp version number.
        /// </summary>
        /// <returns>
        /// The libwebp version number.
        /// </returns>
        internal static int GetLibWebPVersion()
        {
            int version;

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                version = WebP_x64.GetLibWebPVersion();
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                version = WebP_ARM64.GetLibWebPVersion();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            return version;
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
        /// Gets the WebP image metadata.
        /// </summary>
        /// <param name="data">The input image data.</param>
        /// <returns>The image metadata.</returns>
        internal static unsafe DecoderMetadata WebPGetImageMetadata(byte[] data)
        {
            DecoderMetadata metadata = new();

            IDecoderMetadataNative native = metadata;
            WebPSetDecoderMetadata callback = native.SetDecoderMetadata;
            bool result;

            fixed (byte* ptr = data)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    result = WebP_x64.WebPGetImageMetadata(ptr, new UIntPtr((ulong)data.Length), callback);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    result = WebP_ARM64.WebPGetImageMetadata(ptr, new UIntPtr((ulong)data.Length), callback);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }

            GC.KeepAlive(callback);

            if (!result)
            {
                native.CallbackError.Throw();
            }

            return metadata;
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
        /// <param name="options">The encode parameters.</param>
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
            EncoderOptions options,
            EncoderMetadata metadata,
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
                retVal = WebP_x64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, options, metadata, callback);
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                retVal = WebP_ARM64.WebPSave(writeImageCallback, input.Scan0.Pointer, input.Width, input.Height, input.Stride, options, metadata, callback);
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
    }
}
