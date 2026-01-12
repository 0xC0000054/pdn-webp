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

// The progress callback function.
// Returns true if encoding should continue, or false to abort the encoding process.
typedef bool(__stdcall* ProgressFn)(int progress);

// The write image callback.
// This saves memory when writing large images by allowing the caller to read the image in chunks from
// the WebPMemoryWriter's buffer instead requiring that new memory be allocated to store the entire image.
typedef WebPStatus(__stdcall* WriteImageFn)(const uint8_t* image, const size_t imageSize);

typedef struct EncoderOptions
{
    float quality;
    int effort;
    int preset;
    bool lossless;
}EncoderOptions;

// This must be kept in sync with the Native structure in MetadataCustomMarshaler.cs.
typedef struct EncoderMetadata
{
    uint8_t* iccProfile;
    size_t iccProfileSize;
    uint8_t* exif;
    size_t exifSize;
    uint8_t* xmp;
    size_t xmpSize;
}EncoderMetadata;

namespace WebPEncoder
{
    WebPStatus Encode(
        const WriteImageFn writeImageCallback,
        const void* bitmap,
        const int width,
        const int height,
        const int stride,
        const EncoderOptions* encodeOptions,
        const EncoderMetadata* metadata,
        ProgressFn progressCallback);
}