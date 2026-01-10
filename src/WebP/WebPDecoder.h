////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

#pragma once

#include "Common.h"

// The create image callback.
// Returns a null pointer on error.
typedef void* (__stdcall* CreateImageFn)(int width, int height, size_t& outImageDataSize, int& outStride);

enum class MetadataType : int32_t
{
    ColorProfile = 0,
    EXIF,
    XMP
};

// The set decoder metadata callback function.
// Returns true if successful, false otherwise.
typedef bool(__stdcall* SetDecoderMetadataFn)(const uint8_t* data, size_t size, MetadataType type);

namespace WebPDecoder
{
    WebPStatus __stdcall Decode(
        const uint8_t* data,
        size_t dataSize,
        const CreateImageFn createImageCallback,
        const SetDecoderMetadataFn setMetadataCallback);
}