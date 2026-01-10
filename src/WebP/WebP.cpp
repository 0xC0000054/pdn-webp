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

#include "WebP.h"
#include "decode.h"

DLLEXPORT int __stdcall GetLibWebPVersion()
{
    // Each libwebp API set has its own method to get the version number.
    // All of the version numbers should be identical for a specific libwebp release,
    // so we use the decoder version number;
    return WebPGetDecoderVersion();
}

WebPStatus __stdcall WebPLoad(
    const uint8_t* data,
    size_t dataSize,
    const CreateImageFn createImageCallback,
    const SetDecoderMetadataFn setMetadataCallback)
{
    return WebPDecoder::Decode(
        data,
        dataSize,
        createImageCallback,
        setMetadataCallback);
}

WebPStatus __stdcall WebPSave(
    const WriteImageFn writeImageCallback,
    const void* bitmap,
    const int width,
    const int height,
    const int stride,
    const EncoderOptions* encodeOptions,
    const EncoderMetadata* metadata,
    ProgressFn progressCallback)
{
    return WebPEncoder::Encode(
        writeImageCallback,
        bitmap,
        width,
        height,
        stride,
        encodeOptions,
        metadata,
        progressCallback);
}
