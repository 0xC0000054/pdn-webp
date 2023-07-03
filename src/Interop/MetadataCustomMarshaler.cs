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
using System.Runtime.InteropServices;

namespace WebPFileType.Interop
{
    internal sealed class MetadataCustomMarshaler : ICustomMarshaler
    {
        // This must be kept in sync with the MetadataParams structure in WebP.h.
        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMetadataParams
        {
            public IntPtr iccProfile;
            public UIntPtr iccProfileSize;
            public IntPtr exif;
            public UIntPtr exifSize;
            public IntPtr xmp;
            public UIntPtr xmpSize;
        }

        private static readonly int NativeMetadataParamsSize = Marshal.SizeOf(typeof(NativeMetadataParams));
        private static readonly MetadataCustomMarshaler instance = new();

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return instance;
        }

        private MetadataCustomMarshaler()
        {
        }

        public void CleanUpManagedData(object ManagedObj)
        {
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            unsafe
            {
                if (pNativeData != IntPtr.Zero)
                {
                    NativeMetadataParams* metadata = (NativeMetadataParams*)pNativeData;

                    if (metadata->iccProfile != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(metadata->iccProfile);
                    }

                    if (metadata->exif != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(metadata->exif);
                    }

                    if (metadata->xmp != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(metadata->xmp);
                    }

                    Marshal.FreeHGlobal(pNativeData);
                }
            }
        }

        public int GetNativeDataSize()
        {
            return NativeMetadataParamsSize;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
            {
                return IntPtr.Zero;
            }

            MetadataParams metadata = (MetadataParams)ManagedObj;

            IntPtr nativeStructure = Marshal.AllocHGlobal(NativeMetadataParamsSize);

            unsafe
            {
                NativeMetadataParams* nativeMetadata = (NativeMetadataParams*)nativeStructure;

                if (metadata.iccProfile != null && metadata.iccProfile.Length > 0)
                {
                    nativeMetadata->iccProfile = Marshal.AllocHGlobal(metadata.iccProfile.Length);
                    Marshal.Copy(metadata.iccProfile, 0, nativeMetadata->iccProfile, metadata.iccProfile.Length);
                    nativeMetadata->iccProfileSize = new UIntPtr((uint)metadata.iccProfile.Length);
                }
                else
                {
                    nativeMetadata->iccProfile = IntPtr.Zero;
                    nativeMetadata->iccProfileSize = UIntPtr.Zero;
                }

                if (metadata.exif != null && metadata.exif.Length > 0)
                {
                    nativeMetadata->exif = Marshal.AllocHGlobal(metadata.exif.Length);
                    Marshal.Copy(metadata.exif, 0, nativeMetadata->exif, metadata.exif.Length);
                    nativeMetadata->exifSize = new UIntPtr((uint)metadata.exif.Length);
                }
                else
                {
                    nativeMetadata->exif = IntPtr.Zero;
                    nativeMetadata->exifSize = UIntPtr.Zero;
                }

                if (metadata.xmp != null && metadata.xmp.Length > 0)
                {
                    nativeMetadata->xmp = Marshal.AllocHGlobal(metadata.xmp.Length);
                    Marshal.Copy(metadata.xmp, 0, nativeMetadata->xmp, metadata.xmp.Length);
                    nativeMetadata->xmpSize = new UIntPtr((uint)metadata.xmp.Length);
                }
                else
                {
                    nativeMetadata->xmp = IntPtr.Zero;
                    nativeMetadata->xmpSize = UIntPtr.Zero;
                }
            }

            return nativeStructure;
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return null;
        }
    }
}
