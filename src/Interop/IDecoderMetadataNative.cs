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

using System.Runtime.ExceptionServices;

namespace WebPFileType.Interop
{
    internal interface IDecoderMetadataNative
    {
        unsafe bool SetDecoderMetadata(nint data, nuint size, MetadataType type);

        ExceptionDispatchInfo CallbackError { get; }
    }
}
