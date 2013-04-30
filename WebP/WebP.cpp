#include <stdio.h>
#include <stdlib.h>
#include <memory.h>
#include "WebP.h"

bool WebPGetDimensions(uint8_t* iData, size_t iDataSize, int *oWidth, int *oHeight)
{
	return (WebPGetInfo(iData, iDataSize, oWidth, oHeight) != 0);
}

void WebPFreeMemory(void *mem)
{
	free(mem);
}

int WebPLoad(uint8_t* data, size_t dataSize, uint8_t** outData, uint32_t outSize, int outStride)
{
	WebPDecoderConfig config;
	WebPDecBuffer* const output_buffer = &config.output;

	if (!WebPInitDecoderConfig(&config))
	{
		return errVersionMismatch;
	}
	
	output_buffer->colorspace = MODE_BGRA;
	output_buffer->is_external_memory = 1;
	output_buffer->u.RGBA.rgba = *outData;
	output_buffer->u.RGBA.size = outSize;
	output_buffer->u.RGBA.stride = outStride;

	
	return WebPDecode(data, dataSize, &config);
}

static ProgressFn progressCallback;

static int progressFunc(int percent, const WebPPicture* picture)
{
	progressCallback(percent);

	return 1;
}


int WebPSave(void **output, size_t* outputSize, void *bitmap, int width, int height, int stride, EncodeParams params, ProgressFn callback)
{
	WebPConfig config;
	WebPPicture pic;
	WebPMemoryWriter wrt;
	progressCallback = NULL;

	if (!WebPConfigPreset(&config, (WebPPreset)params.preset, params.quality) || !WebPPictureInit(&pic)) 
	{
		return errVersionMismatch; // WebP API version mismatch
	}


	if (params.fileSize > 0)
	{
		config.target_size = params.fileSize;
	}

	if (params.preset < WEBP_PRESET_ICON)
	{
		config.filter_strength = params.filterStrength;	
	}
	config.filter_type = params.filterType;
	config.filter_sharpness = params.sharpness;
	config.method = params.method;

	if (params.quality == 100)
	{
		config.lossless = 1;
		pic.use_argb = 1;

		switch (params.preset)
		{
		case WEBP_PRESET_PHOTO:
			config.image_hint = WEBP_HINT_PHOTO; 
			break;
		case WEBP_PRESET_PICTURE:
			config.image_hint = WEBP_HINT_PICTURE; 
			break;
		case WEBP_PRESET_DRAWING:
			config.image_hint = WEBP_HINT_GRAPH; 
			break;
		}
	}
	

	pic.width = width;
	pic.height = height;
	pic.writer = WebPMemoryWrite;
	pic.custom_ptr = &wrt;
		
	WebPMemoryWriterInit(&wrt);

	if (WebPPictureImportBGRA(&pic, (uint8_t*)bitmap, stride) == 0)
	{
		return VP8_ENC_ERROR_OUT_OF_MEMORY; 
	}

	if (callback != NULL)
	{
		progressCallback = callback;
		pic.progress_hook = progressFunc;
	}

	int error = 0;
	if (WebPEncode(&config, &pic) != 0) // C-style Boolean
	{
		*output = wrt.mem;
		*outputSize = wrt.size;
	}
	else
	{	
		free(wrt.mem);
		error = (int)pic.error_code;
	}
	WebPPictureFree(&pic); // free the allocated memory and return the error code if necessary.

	return error;
}

void GetMetaDataSize(uint8_t* data, size_t dataSize,  MetaDataType type, uint32_t *outSize)
{
	*outSize = 0;

	WebPData webpData;
	webpData.bytes = data;
	webpData.size = dataSize;

	WebPDemuxer* demux = WebPDemux(&webpData);
	uint32_t flags = WebPDemuxGetI(demux, WEBP_FF_FORMAT_FLAGS);

	WebPChunkIterator iter;
	memset(&iter, 0, sizeof(WebPChunkIterator));

	switch (type)
	{
	case ColorProfile:
		if ((flags & ICCP_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "ICCP", 1, &iter);
		}
	break;
	case EXIF:
		if ((flags & EXIF_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "EXIF", 1, &iter);
		}
		break;
	case XMP:
		if ((flags & XMP_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "XMP ", 1, &iter);
		}
		break;
	}

	*outSize = (uint32_t)iter.chunk.size;

	WebPDemuxReleaseChunkIterator(&iter);
	WebPDemuxDelete(demux);
}

void ExtractMetaData(uint8_t* data, size_t dataSize, uint8_t** outData, uint32_t outSize, int type)
{
	WebPData webpData;
	webpData.bytes = data;
	webpData.size = dataSize;

	WebPDemuxer* demux = WebPDemux(&webpData);
	uint32_t flags = WebPDemuxGetI(demux, WEBP_FF_FORMAT_FLAGS);

	WebPChunkIterator iter;
	memset(&iter, 0, sizeof(WebPChunkIterator));

	switch (type)
	{
	case ColorProfile:
		if ((flags & ICCP_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "ICCP", 1, &iter);
		}
	break;
	case EXIF:
		if ((flags & EXIF_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "EXIF", 1, &iter);
		}
		break;
	case XMP:
		if ((flags & XMP_FLAG) != 0)
		{
			WebPDemuxGetChunk(demux, "XMP ", 1, &iter);
		}
		break;
	}

	memcpy_s(*outData, outSize, iter.chunk.bytes, outSize);

	WebPDemuxReleaseChunkIterator(&iter);
	WebPDemuxDelete(demux);
}

WebPMuxError SetMetaData(uint8_t* image, size_t imageSize, void** outImage, size_t* outImageSize, MetaDataParams metaData)
{
	WebPMux *mux = WebPMuxNew();

	WebPData imageData;
	imageData.bytes = image;
	imageData.size = imageSize;
	
	WebPMuxError error = WebPMuxSetImage(mux, &imageData, 0);

	if (error == WEBP_MUX_OK)
	{
		WebPData chunkData;

		if (metaData.iccProfileSize > 0)
		{
			chunkData.bytes = metaData.iccProfile;
			chunkData.size = metaData.iccProfileSize;
			
			error = WebPMuxSetChunk(mux, "ICCP", &chunkData, 1);
		}
		
		if (error == WEBP_MUX_OK && metaData.exifSize > 0)
		{
			chunkData.bytes = metaData.exif;
			chunkData.size = metaData.exifSize;
			
			error = WebPMuxSetChunk(mux, "EXIF", &chunkData, 1);
		}
		
		if (error == WEBP_MUX_OK && metaData.xmpSize > 0)
		{
			chunkData.bytes = metaData.xmp;
			chunkData.size = metaData.xmpSize;
			
			error = WebPMuxSetChunk(mux, "XMP ", &chunkData, 1);
		}

		if (error == WEBP_MUX_OK)
		{
			WebPData assembledData;

			error = WebPMuxAssemble(mux, &assembledData);
			WebPMuxDelete(mux);
			mux = NULL;

			if (error == WEBP_MUX_OK)
			{
				*outImageSize = assembledData.size;
				*outImage = malloc(assembledData.size);

				if (*outImage == NULL)
				{
					error = WEBP_MUX_MEMORY_ERROR;
				}

				if (error == WEBP_MUX_OK)
				{
					memcpy_s(*outImage, assembledData.size, assembledData.bytes, assembledData.size);
				}

				WebPDataClear(&assembledData);
			}
		}

	}

	return error;
}