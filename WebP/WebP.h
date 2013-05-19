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

typedef struct EncodeParams
{
	float quality;
	int preset;
	int filterType;
	int filterStrength;
	int sharpness;	
	int method;
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

DLLEXPORT bool WebPGetDimensions(uint8_t* iData, size_t iData_size, int *oWidth, int *oHeight);

DLLEXPORT void WebPFreeMemory(void *mem);

DLLEXPORT int WebPLoad(uint8_t* data, size_t dataSize, uint8_t** outData, uint32_t outSize, int outStride);

DLLEXPORT int WebPSave(void **output, size_t* outputSize, void *iBitmap, int iWidth, int iHeight, int iStride, EncodeParams params, ProgressFn callback);

DLLEXPORT void GetMetaDataSize(uint8_t* data, size_t dataSize,  MetaDataType type, uint32_t *outSize);

DLLEXPORT void ExtractMetaData(uint8_t* data, size_t dataSize, uint8_t* outData, uint32_t outSize, int type);

DLLEXPORT WebPMuxError SetMetaData(uint8_t* image, size_t imageSize, void** outImage, size_t* outImageSize, MetaDataParams metadata);


#define errVersionMismatch -1


#ifdef __cplusplus
}
#endif