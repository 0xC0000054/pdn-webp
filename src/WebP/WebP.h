////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2025 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

#pragma once

#include "decode.h"
#include "encode.h"
#include "mux_types.h"
#include "mux.h"
#include "demux.h"

#ifdef __cplusplus
extern "C" {
#endif

#ifdef WEBP_EXPORTS
#define DLLEXPORT  __declspec(dllexport)
#else
#define DLLEXPORT __declspec(dllimport)
#endif

enum class WebPStatus : int32_t
{
    Ok = 0,
    OutOfMemory,
    InvalidParameter,
    UnsupportedFeature,
    InvalidImage,
    InvalidEncoderConfiguration,
    BadDimension,           // picture has invalid width/height
    PartitionZeroOverflow,  // partition is bigger than 512k
    PartitionOverflow,      // partition is bigger than 16M
    BadWrite,               // error while flushing bytes
    FileTooBig,             // file is bigger than 4G
    UserAbort,
    MetadataEncoding,
    ApiVersionMismatch,
    UnknownError,
    CreateImageCallbackFailed,
    SetMetadataCallbackFailed,
    DecodeFailed,
};

// The progress callback function.
// Returns true if encoding should continue, or false to abort the encoding process.
typedef bool (__stdcall *ProgressFn)(int progress);

// The create image callback.
// Returns a null pointer on error.
typedef void* (__stdcall* CreateImageFn)(int width, int height, size_t& outImageDataSize, int& outStride);

// The write image callback.
// This saves memory when writing large images by allowing the caller to read the image in chunks from
// the WebPMemoryWriter's buffer instead requiring that new memory be allocated to store the entire image.
typedef WebPStatus (__stdcall *WriteImageFn)(const uint8_t* image, const size_t imageSize);

typedef struct EncoderOptions
{
    float quality;
    int preset;
    bool lossless;
}EncoderOptions;

enum class MetadataType : int32_t
{
    ColorProfile = 0,
    EXIF,
    XMP
};

// The set decoder metadata callback function.
// Returns true if successful, false otherwise.
typedef bool(__stdcall* SetDecoderMetadataFn)(const uint8_t* data, size_t size, MetadataType type);

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

#define errVersionMismatch -1
#define errMuxEncodeMetadata -2

#ifdef __cplusplus
}
#endif