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
    internal readonly partial struct ImageInfo
    {
        [CustomMarshaller(typeof(ImageInfo), MarshalMode.ManagedToUnmanagedOut, typeof(Marshaller))]
        public static class Marshaller
        {
            // This must be kept in sync with the ImageInfo structure in WebP.h.
            [StructLayout(LayoutKind.Sequential)]
            public struct Native
            {
                public int width;
                public int height;
                public byte hasAnimation;
            }

            public static ImageInfo ConvertToManaged(Native unmanaged)
                => new(unmanaged.width, unmanaged.height, unmanaged.hasAnimation != 0);
        }
    }
}
