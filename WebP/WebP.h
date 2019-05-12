////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2019 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

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

typedef void (__stdcall *ProgressFn)(int progress);

typedef void (__stdcall *WriteImageFn)(const uint8_t* image, const size_t imageSize);

typedef struct EncodeParams
{
    float quality;
    int preset;
}EncParams;

enum MetadataType
{
    ColorProfile = 0,
    EXIF,
    XMP
};

// This must be kept in sync with the NativeMetadataParams structure in MetadataCustomMarshaller.cs.
typedef struct MetadataParams
{
    uint8_t* iccProfile;
    uint32_t iccProfileSize;
    uint8_t* exif;
    uint32_t exifSize;
    uint8_t* xmp;
    uint32_t xmpSize;
}MetadataParams;

DLLEXPORT bool __stdcall WebPGetDimensions(const uint8_t* iData, size_t iData_size, int* oWidth, int* oHeight);

DLLEXPORT int __stdcall WebPLoad(const uint8_t* data, size_t dataSize, uint8_t* outData, size_t outSize, int outStride);

DLLEXPORT int __stdcall WebPSave(
    const WriteImageFn writeImageCallback,
    const void* iBitmap,
    const int width,
    const int height,
    const int stride,
    const EncodeParams* params,
    const MetadataParams* metadata,
    ProgressFn progressCallback);

DLLEXPORT uint32_t __stdcall GetMetadataSize(const uint8_t* data, size_t dataSize,  MetadataType type);

DLLEXPORT void __stdcall ExtractMetadata(const uint8_t* data, size_t dataSize, uint8_t* outData, uint32_t outSize, MetadataType type);

#define errVersionMismatch -1
#define errMuxEncodeMetaData -2

#ifdef __cplusplus
}
#endif