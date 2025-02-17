////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2025 Nicholas Hayes
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
    internal sealed partial class EncoderMetadata
    {
        [CustomMarshaller(typeof(EncoderMetadata), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
        internal static class Marshaller
        {
            // This must be kept in sync with the MetadataParams structure in WebP.h.
            [StructLayout(LayoutKind.Sequential)]
            public struct Native
            {
                public IntPtr iccProfile;
                public UIntPtr iccProfileSize;
                public IntPtr exif;
                public UIntPtr exifSize;
                public IntPtr xmp;
                public UIntPtr xmpSize;
            }

            public static unsafe Native ConvertToUnmanaged(EncoderMetadata managed)
            {
                Native native = new();

                if (managed.iccProfile.Length > 0)
                {
                    native.iccProfile = Marshal.AllocCoTaskMem(managed.iccProfile.Length);
                    managed.iccProfile.Span.CopyTo(new Span<byte>((byte*)native.iccProfile, managed.iccProfile.Length));
                    native.iccProfileSize = (uint)managed.iccProfile.Length;
                }

                if (managed.exif.Length > 0)
                {
                    native.exif = Marshal.AllocCoTaskMem(managed.exif.Length);
                    managed.exif.Span.CopyTo(new Span<byte>((byte*)native.exif, managed.exif.Length));
                    native.exifSize = (uint)managed.exif.Length;
                }

                if (managed.xmp.Length > 0)
                {
                    native.xmp = Marshal.AllocCoTaskMem(managed.xmp.Length);
                    managed.xmp.Span.CopyTo(new Span<byte>((byte*)native.xmp, managed.xmp.Length));
                    native.xmpSize = (uint)managed.xmp.Length;
                }

                return native;
            }

            public static void Free(Native native)
            {
                if (native.iccProfile != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(native.iccProfile);
                }

                if (native.exif != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(native.exif);
                }

                if (native.xmp != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(native.xmp);
                }
            }
        }
    }
}
