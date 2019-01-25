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

typedef void* (__stdcall *OutputBufferAllocFn)(size_t sizeInBytes);

typedef struct EncodeParams
{
	float quality;
	int preset;
	int method;
	// The following are only used for lossy compression:
	int noiseShaping;
	int filterType;
	int filterStrength;
	int sharpness;
	int fileSize;
}EncParams;

enum MetaDataType
{
	ColorProfile = 0,
	EXIF,
	XMP
};

typedef struct MetaDataParams
{
	uint8_t* iccProfile;
	uint32_t iccProfileSize;
	uint8_t* exif;
	uint32_t exifSize;
	uint8_t* xmp;
	uint32_t xmpSize;
}MetaDataParams;

DLLEXPORT bool __stdcall WebPGetDimensions(const uint8_t* iData, size_t iData_size, int* oWidth, int* oHeight);

DLLEXPORT int __stdcall WebPLoad(const uint8_t* data, size_t dataSize, uint8_t* outData, size_t outSize, int outStride);

DLLEXPORT int __stdcall WebPSave(
	void** output,
	const OutputBufferAllocFn outputAllocator,
	const void* iBitmap,
	const int width,
	const int height,
	const int stride,
	const EncodeParams* params,
	const MetaDataParams* metaData,
	ProgressFn progressCallback);

DLLEXPORT void __stdcall GetMetaDataSize(const uint8_t* data, size_t dataSize,  MetaDataType type, uint32_t* outSize);

DLLEXPORT void __stdcall ExtractMetaData(const uint8_t* data, size_t dataSize, uint8_t* outData, uint32_t outSize, MetaDataType type);

#define errVersionMismatch -1
#define errMuxEncodeMetaData -2

#ifdef __cplusplus
}
#endif