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

namespace WebPFileType.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class MetadataParams
    {
        public byte[] iccProfile;
        public byte[] exif;
        public byte[] xmp;

        public MetadataParams(byte[] iccProfileBytes, byte[] exifBytes, byte[] xmpBytes)
        {
            if (iccProfileBytes != null)
            {
                iccProfile = (byte[])iccProfileBytes.Clone();
            }

            if (exifBytes != null)
            {
                exif = (byte[])exifBytes.Clone();
            }

            if (xmpBytes != null)
            {
                xmp = (byte[])xmpBytes.Clone();
            }
        }
    }
}
