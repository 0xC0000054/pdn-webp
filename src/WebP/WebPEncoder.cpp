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

#include "WebPEncoder.h"
#include "encode.h"
#include "mux_types.h"
#include "mux.h"
#include "scoped.h"

namespace
{
    bool HasTransparency(const void* data, int width, int height, int stride)
    {
        const uint8_t* scan0 = reinterpret_cast<const uint8_t*>(data);

        for (int y = 0; y < height; y++)
        {
            const uint8_t* ptr = scan0 + (static_cast<int64_t>(y) * stride);
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

    int ProgressReport(int percent, const WebPPicture* picture)
    {
        ProgressFn callback = reinterpret_cast<ProgressFn>(picture->user_data);
        bool continueProcessing = callback(percent);

        return continueProcessing ? 1 : 0;
    }

    WebPStatus EncodeImageMetadata(
        const uint8_t* image,
        const size_t imageSize,
        const EncoderMetadata* metadata,
        const WriteImageFn writeImageCallback)
    {
        if (image == nullptr || metadata == nullptr || writeImageCallback == nullptr)
        {
            return WebPStatus::InvalidParameter;
        }

        ScopedWebPMux mux(WebPMuxNew());
        if (mux == nullptr)
        {
            return WebPStatus::OutOfMemory;
        }

        WebPStatus status = WebPStatus::Ok;

        WebPData imageData{};
        imageData.bytes = image;
        imageData.size = imageSize;

        WebPMuxError muxError = WebPMuxSetImage(mux.get(), &imageData, 0);

        if (muxError == WEBP_MUX_OK)
        {
            WebPData chunkData{};

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
                    status = writeImageCallback(assembler.GetBuffer(), assembler.GetBufferSize());
                }
            }
        }

        if (muxError != WEBP_MUX_OK && status == WebPStatus::Ok)
        {
            switch (muxError)
            {
            case WEBP_MUX_MEMORY_ERROR:
                status = WebPStatus::OutOfMemory;
                break;

            case WEBP_MUX_NOT_FOUND:
            case WEBP_MUX_INVALID_ARGUMENT:
            case WEBP_MUX_BAD_DATA:
            case WEBP_MUX_NOT_ENOUGH_DATA:
            default:
                status = WebPStatus::MetadataEncoding;
                break;
            }
        }

        return status;
    }
}

WebPStatus WebPEncoder::Encode(
    const WriteImageFn writeImageCallback,
    const void* bitmap,
    const int width,
    const int height,
    const int stride,
    const EncoderOptions* encodeOptions,
    const EncoderMetadata* metadata,
    ProgressFn progressCallback)
{
    if (writeImageCallback == nullptr || bitmap == nullptr || encodeOptions == nullptr)
    {
        return WebPStatus::InvalidParameter;
    }

    WebPConfig config;
    ScopedWebPPicture pic;
    ScopedWebPMemoryWriter wrt;

    if (pic == nullptr || wrt == nullptr)
    {
        return WebPStatus::OutOfMemory;
    }

    if (!WebPConfigPreset(&config, static_cast<WebPPreset>(encodeOptions->preset), encodeOptions->quality) || !pic.IsInitalized())
    {
        return WebPStatus::ApiVersionMismatch; // WebP API version mismatch
    }

    config.method = 6; // 6 is the highest quality encoding
    config.thread_level = 1;

    if (encodeOptions->lossless)
    {
        config.lossless = 1;
        config.exact = 1; // Preserve color values of invisible/transparent pixels like the built-in PNG output of PDN
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
            return WebPStatus::OutOfMemory;
        }
    }
    else
    {
        // If the image does not have any transparency import using the BGRX method which will ignore the alpha channel.
        if (WebPPictureImportBGRX(pic.Get(), reinterpret_cast<const uint8_t*>(bitmap), stride) == 0)
        {
            return WebPStatus::OutOfMemory;
        }
    }

    if (progressCallback != nullptr)
    {
        pic->user_data = progressCallback;
        pic->progress_hook = ProgressReport;
    }

    WebPStatus status = WebPStatus::Ok;
    if (WebPEncode(&config, pic.Get()) != 0) // C-style Boolean
    {
        if (metadata != nullptr)
        {
            status = EncodeImageMetadata(wrt.GetBuffer(), wrt.GetBufferSize(), metadata, writeImageCallback);
        }
        else
        {
            status = writeImageCallback(wrt.GetBuffer(), wrt.GetBufferSize());
        }
    }
    else
    {
        switch (pic->error_code)
        {
        case VP8_ENC_OK:
            status = WebPStatus::Ok;
            break;
        case VP8_ENC_ERROR_OUT_OF_MEMORY:
        case VP8_ENC_ERROR_BITSTREAM_OUT_OF_MEMORY:
            status = WebPStatus::OutOfMemory;
            break;
        case VP8_ENC_ERROR_NULL_PARAMETER:
            status = WebPStatus::InvalidParameter;
            break;
        case VP8_ENC_ERROR_INVALID_CONFIGURATION:
            status = WebPStatus::InvalidEncoderConfiguration;
            break;
        case VP8_ENC_ERROR_BAD_DIMENSION:
            status = WebPStatus::BadDimension;
            break;
        case VP8_ENC_ERROR_PARTITION0_OVERFLOW:
            status = WebPStatus::PartitionZeroOverflow;
            break;
        case VP8_ENC_ERROR_PARTITION_OVERFLOW:
            status = WebPStatus::PartitionOverflow;
            break;
        case VP8_ENC_ERROR_BAD_WRITE:
            status = WebPStatus::BadWrite;
            break;
        case VP8_ENC_ERROR_FILE_TOO_BIG:
            status = WebPStatus::FileTooBig;
            break;
        case VP8_ENC_ERROR_USER_ABORT:
            status = WebPStatus::UserAbort;
            break;
        default:
            status = WebPStatus::UnknownError;
            break;
        }
    }

    return status;
}
