////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2024 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WebPFileType.Interop
{
    [NativeMarshalling(typeof(Marshaller))]
    internal sealed partial class EncoderMetadata
    {
        public ReadOnlyMemory<byte> iccProfile;
        public ReadOnlyMemory<byte> exif;
        public ReadOnlyMemory<byte> xmp;

        public EncoderMetadata(byte[]? iccProfileBytes, byte[]? exifBytes, byte[]? xmpBytes)
        {
            iccProfile = iccProfileBytes;
            exif = exifBytes;
            xmp = xmpBytes;
        }
    }
}
