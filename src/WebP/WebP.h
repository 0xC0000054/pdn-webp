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

#include "WebPDecoder.h"
#include "WebPEncoder.h"

#ifdef __cplusplus
extern "C" {
#endif

#ifdef WEBP_EXPORTS
#define DLLEXPORT  __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllimport)
#endif

DLLEXPORT int __stdcall GetLibWebPVersion();

DLLEXPORT WebPStatus __stdcall WebPLoad(
    const uint8_t* data,
    size_t dataSize,
    const CreateImageFn createImageCallback,
    const SetDecoderMetadataFn setMetadataCallback);

DLLEXPORT WebPStatus __stdcall WebPSave(
    const WriteImageFn writeImageCallback,
    const void* bitmap,
    const int width,
    const int height,
    const int stride,
    const EncoderOptions* encodeOptions,
    const EncoderMetadata* metadata,
    ProgressFn progressCallback);

#ifdef __cplusplus
}
#endif