////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2020 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

#include <memory>
#include "WebP.h"
#include "scoped.h"

bool __stdcall WebPGetDimensions(const uint8_t* data, size_t dataSize, int* width, int* height)
{
    return (WebPGetInfo(data, dataSize, width, height) != 0);
}

int __stdcall WebPLoad(const uint8_t* data, size_t dataSize, uint8_t* outData, size_t outSize, int outStride)
{
    WebPDecoderConfig config;

    if (!WebPInitDecoderConfig(&config))
    {
        return errVersionMismatch;
    }

    config.output.colorspace = MODE_BGRA;
    config.output.is_external_memory = 1;
    config.output.u.RGBA.rgba = outData;
    config.output.u.RGBA.size = outSize;
    config.output.u.RGBA.stride = outStride;

    VP8StatusCode status = WebPDecode(data, dataSize, &config);

    WebPFreeDecBuffer(&config.output);

    return status;
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
            {
                return true;
            }

            ptr += 4;
        }
    }

    return false;
}

static int ProgressReport(int percent, const WebPPicture* picture)
{
    ProgressFn callback = reinterpret_cast<ProgressFn>(picture->user_data);
    bool continueProcessing = callback(percent);

    return continueProcessing ? 1 : 0;
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

    ScopedWebPMux mux(WebPMuxNew());
    if (mux == nullptr)
    {
        return VP8_ENC_ERROR_OUT_OF_MEMORY;
    }

    int encodeError = VP8_ENC_OK;

    WebPData imageData;
    imageData.bytes = image;
    imageData.size = imageSize;

    WebPMuxError muxError = WebPMuxSetImage(mux.get(), &imageData, 0);

    if (muxError == WEBP_MUX_OK)
    {
        WebPData chunkData;

        if (metadata->iccProfileSize > 0)
        {
            chunkData.bytes = metadata->iccProfile;
            chunkData.size = metadata->iccProfileSize;

            muxError = WebPMuxSetChunk(mux.get(), "ICCP", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK && metadata->exifSize > 0)
        {
            chunkData.bytes = metadata->exif;
            chunkData.size = metadata->exifSize;

            muxError = WebPMuxSetChunk(mux.get(), "EXIF", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK && metadata->xmpSize > 0)
        {
            chunkData.bytes = metadata->xmp;
            chunkData.size = metadata->xmpSize;

            muxError = WebPMuxSetChunk(mux.get(), "XMP ", &chunkData, 1);
        }

        if (muxError == WEBP_MUX_OK)
        {
            ScopedWebPMuxAssembler assembler(mux.get());

            muxError = assembler.GetStatus();

            if (muxError == WEBP_MUX_OK)
            {
                encodeError = writeImageCallback(assembler.GetBuffer(), assembler.GetBufferSize());
            }
        }
    }

    if (muxError != WEBP_MUX_OK && encodeError == VP8_ENC_OK)
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
    const EncodeParams* encodeOptions,
    const MetadataParams* metadata,
    ProgressFn callback)
{
    if (writeImageCallback == nullptr || bitmap == nullptr || encodeOptions == nullptr)
    {
        return VP8_ENC_ERROR_NULL_PARAMETER;
    }

    WebPConfig config;
    ScopedWebPPicture pic;
    ScopedWebPMemoryWriter wrt;

    if (pic == nullptr || wrt == nullptr)
    {
        return VP8_ENC_ERROR_OUT_OF_MEMORY;
    }

    if (!WebPConfigPreset(&config, static_cast<WebPPreset>(encodeOptions->preset), encodeOptions->quality) || !pic.IsInitalized())
    {
        return errVersionMismatch; // WebP API version mismatch
    }

    config.method = 6; // 6 is the highest quality encoding
    config.thread_level = 1;

    if (encodeOptions->quality == 100)
    {
        config.lossless = 1;
        pic->use_argb = 1;

        switch (encodeOptions->preset)
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

    pic->width = width;
    pic->height = height;

    pic->writer = WebPMemoryWrite;
    pic->custom_ptr = wrt.Get();

    if (HasTransparency(bitmap, width, height, stride))
    {
        if (WebPPictureImportBGRA(pic.Get(), reinterpret_cast<const uint8_t*>(bitmap), stride) == 0)
        {
            return VP8_ENC_ERROR_OUT_OF_MEMORY;
        }
    }
    else
    {
        // If the image does not have any transparency import using the BGRX method which will ignore the alpha channel.
        if (WebPPictureImportBGRX(pic.Get(), reinterpret_cast<const uint8_t*>(bitmap), stride) == 0)
        {
            return VP8_ENC_ERROR_OUT_OF_MEMORY;
        }
    }

    if (callback != nullptr)
    {
        pic->user_data = callback;
        pic->progress_hook = ProgressReport;
    }

    int error = VP8_ENC_OK;
    if (WebPEncode(&config, pic.Get()) != 0) // C-style Boolean
    {
        if (metadata != nullptr)
        {
            error = EncodeImageMetadata(wrt.GetBuffer(), wrt.GetBufferSize(), metadata, writeImageCallback);
        }
        else
        {
            error = writeImageCallback(wrt.GetBuffer(), wrt.GetBufferSize());
        }
    }
    else
    {
        error = static_cast<int>(pic->error_code);
    }

    return error;
}

uint32_t __stdcall GetMetadataSize(const uint8_t* data, size_t dataSize, MetadataType type)
{
    uint32_t outSize = 0;

    WebPData webpData;
    webpData.bytes = data;
    webpData.size = dataSize;

    ScopedWebPDemuxer demux(WebPDemux(&webpData));
    if (demux != nullptr)
    {
        uint32_t flags = WebPDemuxGetI(demux.get(), WEBP_FF_FORMAT_FLAGS);

        WebPChunkIterator iter;
        memset(&iter, 0, sizeof(WebPChunkIterator));
        int result = 0;

        switch (type)
        {
        case ColorProfile:
            if ((flags & ICCP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "ICCP", 1, &iter);
            }
        break;
        case EXIF:
            if ((flags & EXIF_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "EXIF", 1, &iter);
            }
            break;
        case XMP:
            if ((flags & XMP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "XMP ", 1, &iter);
            }
            break;
        }

        if (result != 0)
        {
            outSize = static_cast<uint32_t>(iter.chunk.size);
        }

        WebPDemuxReleaseChunkIterator(&iter);
    }

    return outSize;
}

void __stdcall ExtractMetadata(const uint8_t* data, size_t dataSize, uint8_t* outData, uint32_t outSize, MetadataType type)
{
    WebPData webpData;
    webpData.bytes = data;
    webpData.size = dataSize;

    ScopedWebPDemuxer demux(WebPDemux(&webpData));
    if (demux != nullptr)
    {
        uint32_t flags = WebPDemuxGetI(demux.get(), WEBP_FF_FORMAT_FLAGS);

        WebPChunkIterator iter;
        memset(&iter, 0, sizeof(WebPChunkIterator));
        int result = 0;

        switch (type)
        {
        case ColorProfile:
            if ((flags & ICCP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "ICCP", 1, &iter);
            }
        break;
        case EXIF:
            if ((flags & EXIF_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "EXIF", 1, &iter);
            }
            break;
        case XMP:
            if ((flags & XMP_FLAG) != 0)
            {
                result = WebPDemuxGetChunk(demux.get(), "XMP ", 1, &iter);
            }
            break;
        }

        if (result != 0)
        {
            memcpy_s(outData, outSize, iter.chunk.bytes, iter.chunk.size);
        }

        WebPDemuxReleaseChunkIterator(&iter);
    }
}