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

#include <memory.h>
#include "WebP.h"

bool __stdcall WebPGetDimensions(const uint8_t* iData, size_t iDataSize, int* oWidth, int* oHeight)
{
    return (WebPGetInfo(iData, iDataSize, oWidth, oHeight) != 0);
}

int __stdcall WebPLoad(const uint8_t* data, size_t dataSize, uint8_t* outData, size_t outSize, int outStride)
{
    WebPDecoderConfig config;
    WebPDecBuffer* const output_buffer = &config.output;

    if (!WebPInitDecoderConfig(&config))
    {
        return errVersionMismatch;
    }

    output_buffer->colorspace = MODE_BGRA;
    output_buffer->is_external_memory = 1;
    output_buffer->u.RGBA.rgba = outData;
    output_buffer->u.RGBA.size = outSize;
    output_buffer->u.RGBA.stride = outStride;

    return WebPDecode(data, dataSize, &config);
}

static bool HasTransparency(const void* data, int width, int height, int stride)
{
    const uint8_t* scan0 = reinterpret_cast<const uint8_t*>(data);

    for (int y = 0; y < height; y++)
    {
        const uint8_t* ptr = scan0 + (y * stride);
        for (int x = 0; x < width; x++)
        {
            if (ptr[3] < 255)
                return true;

            ptr += 4;
        }
    }

    return false;
}

static int ProgressReport(int percent, const WebPPicture* picture)
{
    ProgressFn callback = reinterpret_cast<ProgressFn>(picture->user_data);
    callback(percent);

    return 1;
}

static int EncodeImageMetadata(
    const uint8_t* image,
    const size_t imageSize,
    const MetadataParams* metadata,
    const WriteImageFn writeImageCallback)
{
    if (image == nullptr || metadata == nullptr || writeImageCallback == nullptr)
    {
        return VP8_ENC_ERROR_NULL_PARAMETER;
    }

    WebPMux* mux = WebPMuxNew();
    if (mux == nullptr)
    {
        return VP8_ENC_ERROR_OUT_OF_MEMORY;
    }

    WebPData imageData;
    imageData.bytes = image;
    imageData.size = imageSize;

    WebPMuxError muxError = WebPMuxSetImage(mux, &imageData, 0);

    if (muxError == WEBP_MUX_OK)
    {
        WebPData chunkData;

        if (metadata->iccProfileSize > 0)
        {
            chunkData.bytes = metadata->iccProfile;
            chunkData.size = metadata->iccProfileSize;

            muxError = WebPMuxSetChunk(mux, "ICCP", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK && metadata->exifSize > 0)
        {
            chunkData.bytes = metadata->exif;
            chunkData.size = metadata->exifSize;

            muxError = WebPMuxSetChunk(mux, "EXIF", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK && metadata->xmpSize > 0)
        {
            chunkData.bytes = metadata->xmp;
            chunkData.size = metadata->xmpSize;

            muxError = WebPMuxSetChunk(mux, "XMP ", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK)
        {
            WebPData assembledData;

            muxError = WebPMuxAssemble(mux, &assembledData);

            if (muxError == WEBP_MUX_OK)
            {
                writeImageCallback(assembledData.bytes, assembledData.size);

                WebPDataClear(&assembledData);
            }
        }
    }
    WebPMuxDelete(mux);
    mux = nullptr;

    int encodeError = VP8_ENC_OK;

    if (muxError != WEBP_MUX_OK)
    {
        switch (muxError)
        {
        case WEBP_MUX_MEMORY_ERROR:
            encodeError = VP8_ENC_ERROR_OUT_OF_MEMORY;
            break;

        case WEBP_MUX_NOT_FOUND:
        case WEBP_MUX_INVALID_ARGUMENT:
        case WEBP_MUX_BAD_DATA:
        case WEBP_MUX_NOT_ENOUGH_DATA:
        default:
            encodeError = errMuxEncodeMetadata;
            break;
        }
    }

    return encodeError;
}

int __stdcall WebPSave(
    const WriteImageFn writeImageCallback,
    const void* bitmap,
    const int width,
    const int height,
    const int stride,
    const EncodeParams* params,
    const MetadataParams* metadata,
    ProgressFn callback)
{
    if (writeImageCallback == nullptr)
    {
        return VP8_ENC_ERROR_NULL_PARAMETER;
    }

    WebPConfig config;
    WebPPicture pic;
    WebPMemoryWriter wrt;

    if (!WebPConfigPreset(&config, static_cast<WebPPreset>(params->preset), params->quality) || !WebPPictureInit(&pic))
    {
        return errVersionMismatch; // WebP API version mismatch
    }

    config.method = 6; // 6 is the highest quality encoding
    config.thread_level = 1;

    if (params->quality == 100)
    {
        config.lossless = 1;
        pic.use_argb = 1;

        switch (params->preset)
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

    WebPMemoryWriterInit(&wrt);
    pic.writer = WebPMemoryWrite;
    pic.custom_ptr = &wrt;

    if (HasTransparency(bitmap, width, height, stride))
    {
        if (WebPPictureImportBGRA(&pic, reinterpret_cast<const uint8_t*>(bitmap), stride) == 0)
        {
            return VP8_ENC_ERROR_OUT_OF_MEMORY;
        }
    }
    else
    {
        // If the image does not have any transparency import using the BGRX method which will ignore the alpha channel.
        if (WebPPictureImportBGRX(&pic, reinterpret_cast<const uint8_t*>(bitmap), stride) == 0)
        {
            return VP8_ENC_ERROR_OUT_OF_MEMORY;
        }
    }

    if (callback != nullptr)
    {
        pic.user_data = callback;
        pic.progress_hook = ProgressReport;
    }

    int error = VP8_ENC_OK;
    if (WebPEncode(&config, &pic) != 0) // C-style Boolean
    {
        if (metadata != nullptr)
        {
            error = EncodeImageMetadata(wrt.mem, wrt.size, metadata, writeImageCallback);
        }
        else
        {
            writeImageCallback(wrt.mem, wrt.size);
        }
    }
    else
    {
        error = static_cast<int>(pic.error_code);
    }
    WebPMemoryWriterClear(&wrt);
    WebPPictureFree(&pic); // free the allocated memory and return the error code if necessary.

    return error;
}

uint32_t __stdcall GetMetadataSize(const uint8_t* data, size_t dataSize, MetadataType type)
{
    uint32_t outSize = 0;

    WebPData webpData;
    webpData.bytes = data;
    webpData.size = dataSize;

    WebPDemuxer* demux = WebPDemux(&webpData);
    if (demux != nullptr)
    {
        uint32_t flags = WebPDemuxGetI(demux, WEBP_FF_FORMAT_FLAGS);

        WebPChunkIterator iter;
        memset(&iter, 0, sizeof(WebPChunkIterator));
        int result = 0;

        switch (type)
        {
        case ColorProfile:
            if ((flags & ICCP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "ICCP", 1, &iter);
            }
        break;
        case EXIF:
            if ((flags & EXIF_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "EXIF", 1, &iter);
            }
            break;
        case XMP:
            if ((flags & XMP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "XMP ", 1, &iter);
            }
            break;
        }

        if (result != 0)
        {
            outSize = static_cast<uint32_t>(iter.chunk.size);
        }

        WebPDemuxReleaseChunkIterator(&iter);
        WebPDemuxDelete(demux);
    }

    return outSize;
}

void __stdcall ExtractMetadata(const uint8_t* data, size_t dataSize, uint8_t* outData, uint32_t outSize, MetadataType type)
{
    WebPData webpData;
    webpData.bytes = data;
    webpData.size = dataSize;

    WebPDemuxer* demux = WebPDemux(&webpData);
    if (demux != nullptr)
    {
        uint32_t flags = WebPDemuxGetI(demux, WEBP_FF_FORMAT_FLAGS);

        WebPChunkIterator iter;
        memset(&iter, 0, sizeof(WebPChunkIterator));
        int result = 0;

        switch (type)
        {
        case ColorProfile:
            if ((flags & ICCP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "ICCP", 1, &iter);
            }
        break;
        case EXIF:
            if ((flags & EXIF_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "EXIF", 1, &iter);
            }
            break;
        case XMP:
            if ((flags & XMP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux, "XMP ", 1, &iter);
            }
            break;
        }

        if (result != 0)
        {
            memcpy_s(outData, outSize, iter.chunk.bytes, outSize);
        }

        WebPDemuxReleaseChunkIterator(&iter);
        WebPDemuxDelete(demux);
    }
}