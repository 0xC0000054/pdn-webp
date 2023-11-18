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
        /// Gets the libwebp version.
        /// </summary>
        /// <returns>
        /// The libwebp version.
        /// </returns>
        internal static Version GetLibWebPVersion()
        {
            int packedVersion;

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                packedVersion = WebP_x64.GetLibWebPVersion();
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                packedVersion = WebP_ARM64.GetLibWebPVersion();
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            int major = (packedVersion >> 16) & 0xff;
            int minor = (packedVersion >> 8) & 0xff;
            int build = packedVersion & 0xff;

            return new Version(major, minor, build);
        }

        /// <summary>
        /// The WebP load function.
        /// </summary>
        /// <param name="webpBytes">The input image data</param>
        /// <exception cref="ArgumentNullException"><paramref name="webpBytes"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">Insufficient memory to load the WebP image.</exception>
        /// <exception cref="WebPException">
        /// The WebP image is invalid.
        /// -or-
        /// A native API parameter is invalid.
        /// </exception>
        internal static unsafe (Surface, DecoderMetadata) WebPLoad(byte[] webpBytes)
        {
            ArgumentNullException.ThrowIfNull(webpBytes, nameof(webpBytes));

            WebPStatus status;

            DecoderCreateImage createImage = new();
            DecoderMetadata metadata = new();

            IDecoderMetadataNative nativeDecoderMetadata = metadata;
            WebPCreateImage createImageCallback = createImage.CreateImage;
            WebPSetDecoderMetadata setMetadataCallback = nativeDecoderMetadata.SetDecoderMetadata;

            fixed (byte* ptr = webpBytes)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    status = WebP_x64.WebPLoad(ptr, new UIntPtr((uint)webpBytes.Length), createImageCallback, setMetadataCallback);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    status = WebP_ARM64.WebPLoad(ptr, new UIntPtr((uint)webpBytes.Length), createImageCallback, setMetadataCallback);
                }
                else
                {
                    throw new PlatformNotSupportedException();
                }
            }

            GC.KeepAlive(createImageCallback);
            GC.KeepAlive(setMetadataCallback);

            if (status != WebPStatus.Ok)
            {
                switch (status)
                {
                    case WebPStatus.OutOfMemory:
                        throw new OutOfMemoryException();
                    case WebPStatus.InvalidParameter:
                        throw new WebPException(string.Format(CultureInfo.InvariantCulture, Resources.InvalidParameterFormat, nameof(WebPLoad)));
                    case WebPStatus.UnsupportedFeature:
                        throw new WebPException(Resources.UnsupportedWebPFeature);
                    case WebPStatus.CreateImageCallbackFailed:
                        createImage.CallbackErrorInfo!.Throw();
                        break;
                    case WebPStatus.SetMetadataCallbackFailed:
                        nativeDecoderMetadata.CallbackError!.Throw();
                        break;
                    case WebPStatus.DecodeFailed:
                        throw new WebPException(Resources.DecoderGenericError);
                    case WebPStatus.AnimatedImagesNotSupported:
                        throw new WebPException(Resources.AnimatedWebPNotSupported);
                    default:
                        throw new WebPException(Resources.InvalidWebPImage);
                }
            }

            return (createImage.GetSurface()!, metadata);
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
            EncoderMetadata? metadata,
            WebPReportProgress? callback)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StreamIOHandler handler = new(output);
            WebPWriteImage writeImageCallback = handler.WriteImageCallback;

            WebPStatus retVal = WebPStatus.Ok;

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

            if (retVal != WebPStatus.Ok)
            {
                switch (retVal)
                {
                    case WebPStatus.OutOfMemory:
                        throw new OutOfMemoryException(Resources.InsufficientMemoryOnSave);
                    case WebPStatus.FileTooBig:
                        throw new WebPException(Resources.EncoderFileTooBig);
                    case WebPStatus.ApiVersionMismatch:
                        throw new WebPException(Resources.ApiVersionMismatch);
                    case WebPStatus.MetadataEncoding:
                        throw new WebPException(Resources.EncoderMetadataError);
                    case WebPStatus.UserAbort:
                        throw new OperationCanceledException();
                    case WebPStatus.BadDimension:
                        throw new WebPException(Resources.InvalidImageDimensions);
                    case WebPStatus.InvalidParameter:
                        throw new WebPException(Resources.EncoderNullParameter);
                    case WebPStatus.InvalidConfiguration:
                        throw new WebPException(Resources.EncoderInvalidConfiguration);
                    case WebPStatus.PartitionZeroOverflow:
                        throw new WebPException(Resources.EncoderPartitionZeroOverflow);
                    case WebPStatus.PartitionOverflow:
                        throw new WebPException(Resources.EncoderPartitionOverflow);
                    case WebPStatus.BadWrite:
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
