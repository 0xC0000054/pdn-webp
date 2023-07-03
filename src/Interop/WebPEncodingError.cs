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

namespace WebPFileType.Interop
{
    internal enum WebPEncodingError : int
    {
        MetadataEncoding = -2,
        ApiVersionMismatch = -1,
        Ok = 0,
        OutOfMemory = 1,           // memory error allocating objects
        BitStreamOutOfMemory = 2, // memory error while flushing bits
        NullParameter = 3,         // a pointer parameter is NULL
        InvalidConfiguration = 4,   // configuration is invalid
        BadDimension = 5,           // picture has invalid width/height
        PartitionZeroOverflow = 6,     // partition is bigger than 512k
        PartitionOverflow = 7,      // partition is bigger than 16M
        BadWrite = 8,               // error while flushing bytes
        FileTooBig = 9,            // file is bigger than 4G
        UserAbort = 10              // abort request by user
    }
}
