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
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using WebPFileType.Exif;

namespace WebPFileType.Interop
{
    internal sealed class DecoderMetadata : IDecoderMetadataNative
    {
        private byte[] iccProfile;
        private byte[] exif;
        private byte[] xmp;
        private ExceptionDispatchInfo callbackErrorInfo;
        private readonly Lazy<ExifValueCollection> parsedExifData;

        public DecoderMetadata()
        {
            iccProfile = null;
            exif = null;
            xmp = null;
            callbackErrorInfo = null;
            parsedExifData = new Lazy<ExifValueCollection>(ParseExifData);
        }

        unsafe bool IDecoderMetadataNative.SetDecoderMetadata(nint data, nuint size, MetadataType type)
        {
            bool result = true;

            if (data != 0 && size > 0 && size <= int.MaxValue)
            {
                try
                {
                    Span<byte> nativeData = new((byte*)data, (int)size);

                    switch (type)
                    {
                        case MetadataType.ColorProfile:
                            iccProfile ??= nativeData.ToArray();
                            break;
                        case MetadataType.EXIF:
                            exif ??= nativeData.ToArray();
                            break;
                        case MetadataType.XMP:
                            xmp ??= nativeData.ToArray();
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported {nameof(MetadataType)}: {type}.");
                    }
                }
                catch (Exception ex)
                {
                    callbackErrorInfo = ExceptionDispatchInfo.Capture(ex);
                    result = false;
                }
            }

            return result;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ExceptionDispatchInfo IDecoderMetadataNative.CallbackError => callbackErrorInfo;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ExifValueCollection Exif => parsedExifData.Value;

        public byte[] GetColorProfileBytes() => iccProfile;

        public byte[] GetXmpBytes() => xmp;

        private ExifValueCollection ParseExifData()
        {
            ExifValueCollection data = null;

            if (exif != null)
            {
                data = ExifParser.Parse(exif);
            }

            return data;
        }
    }
}
