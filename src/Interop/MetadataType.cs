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

namespace WebPFileType.Interop
{
    internal enum MetadataType : int
    {
        ColorProfile = 0,
        EXIF,
        XMP
    }
}
