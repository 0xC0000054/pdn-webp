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
    internal enum VP8StatusCode : int
    {
        Ok = 0,
        OutOfMemory,
        InvalidParam,
        BitStreamError,
        UnsupportedFeature,
        Suspended,
        UserAbort,
        NotEnoughData,
    }
}
