﻿////////////////////////////////////////////////////////////////////////
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

namespace WebPFileType.Interop
{
    internal enum WebPStatus : int
    {
        Ok = 0,
        OutOfMemory,
        InvalidParameter,
        UnsupportedFeature,
        InvalidImage,
        InvalidConfiguration,   // configuration is invalid
        BadDimension,           // picture has invalid width/height
        PartitionZeroOverflow,     // partition is bigger than 512k
        PartitionOverflow,      // partition is bigger than 16M
        BadWrite,               // error while flushing bytes
        FileTooBig,            // file is bigger than 4G
        UserAbort,
        MetadataEncoding,
        ApiVersionMismatch,
        UnknownError,
    }
}
