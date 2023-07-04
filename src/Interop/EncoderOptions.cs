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

using System.Runtime.InteropServices.Marshalling;

namespace WebPFileType.Interop
{
    [NativeMarshalling(typeof(Marshaller))]
    internal sealed partial class EncoderOptions
    {
        public float quality;
        public WebPPreset preset;
        public bool lossless;
    }
}
