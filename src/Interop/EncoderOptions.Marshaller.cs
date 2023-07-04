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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace WebPFileType.Interop
{
    internal sealed partial class EncoderOptions
    {
        [CustomMarshaller(typeof(EncoderOptions), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
        public static class Marshaller
        {
            // This must be kept in sync with the EncodeParams structure in WebP.h.
            [StructLayout(LayoutKind.Sequential)]
            public struct Native
            {
                public float quality;
                public int preset;
                public byte lossless;
            }

            public static Native ConvertToUnmanaged(EncoderOptions managed)
            {
                return new Native
                {
                    quality = managed.quality,
                    preset = (int)managed.preset,
                    lossless = (byte)(managed.lossless ? 1 : 0)
                };
            }
        }
    }
}
