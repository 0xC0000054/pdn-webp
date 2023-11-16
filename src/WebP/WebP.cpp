////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2023 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

#include <memory>
#include "WebP.h"
#include "scoped.h"

DLLEXPORT int __stdcall GetLibWebPVersion()
{
    // Each libwebp API set has its own method to get the version number.
    // All of the version numbers should be identical for a specific libwebp release,
    // so we use the decoder version number;
    return WebPGetDecoderVersion();
}

static bool SetDecoderMetadata(const WebPDemuxer* dmux, SetDecoderMetadataFn setMetadata, MetadataType type)
{
    const char* fourcc = nullptr;

    switch (type)
    {
    case MetadataType::ColorProfile:
        fourcc = "ICCP";
        break;
    case MetadataType::EXIF:
        fourcc = "EXIF";
        break;
    case MetadataType::XMP:
        fourcc = "XMP ";
        break;
    }

    bool result = true;

    if (fourcc)
    {
        WebPChunkIterator iter;

        if (WebPDemuxGetChunk(dmux, fourcc, 1, &iter))
        {
            if (!setMetadata(iter.chunk.bytes, iter.chunk.size, type))
            {
                result = false;
            }
            WebPDemuxReleaseChunkIterator(&iter);
        }
    }

    return result;
}

static bool WebPGetImageMetadata(const WebPDemuxer* demux, SetDecoderMetadataFn setMetadata)
{
    uint32_t flags = WebPDemuxGetI(demux, WEBP_FF_FORMAT_FLAGS);

    if ((flags & ICCP_FLAG) != 0)
    {
        if (!SetDecoderMetadata(demux, setMetadata, MetadataType::ColorProfile))
        {
            return false;
        }
    }

    if ((flags & EXIF_FLAG) != 0)
    {
        if (!SetDecoderMetadata(demux, setMetadata, MetadataType::EXIF))
        {
            return false;
        }
    }

    if ((flags & XMP_FLAG) != 0)
    {
        if (!SetDecoderMetadata(demux, setMetadata, MetadataType::XMP))
        {
            return false;
        }
    }

    return true;
}

static WebPStatus DecodeImage(
    const WebPData& data,
    int outWidth,
    int outHeight,
    void* outData,
    size_t outDataSize,
    int outStride)
{
    WebPDecoderConfig config;

    if (!WebPInitDecoderConfig(&config))
    {
        return WebPStatus::ApiVersionMismatch;
    }

    WebPStatus status = WebPStatus::Ok;

    config.output.colorspace = MODE_BGRA;
    config.output.is_external_memory = 1;
    config.output.width = outWidth;
    config.output.height = outHeight;
    config.output.u.RGBA.rgba = static_cast<uint8_t*>(outData);
    config.output.u.RGBA.size = outDataSize;
    config.output.u.RGBA.stride = outStride;

    switch (WebPDecode(data.bytes, data.size, &config))
    {
    case VP8_STATUS_OK:
        break;
    case VP8_STATUS_OUT_OF_MEMORY:
        status = WebPStatus::OutOfMemory;
        break;
    case VP8_STATUS_INVALID_PARAM:
        status = WebPStatus::InvalidParameter;
        break;
    case VP8_STATUS_UNSUPPORTED_FEATURE:
        status = WebPStatus::UnsupportedFeature;
        break;
    case VP8_STATUS_USER_ABORT:
        status = WebPStatus::UserAbort;
        break;
    case VP8_STATUS_BITSTREAM_ERROR:
    case VP8_STATUS_SUSPENDED:
    case VP8_STATUS_NOT_ENOUGH_DATA:
    default:
        status = WebPStatus::InvalidImage;
        break;
    }

    WebPFreeDecBuffer(&config.output);

    return status;
}

WebPStatus __stdcall WebPLoad(
    const uint8_t* data,
    size_t dataSize,
    const CreateImageFn createImageCallback,
    const SetDecoderMetadataFn setMetadataCallback)
{
    if (!data || !createImageCallback || !setMetadataCallback)
    {
        return WebPStatus::InvalidParameter;
    }

    WebPData webpData{};
    webpData.bytes = data;
    webpData.size = dataSize;

    ScopedWebPDemuxer demux(WebPDemux(&webpData));

    if (!demux)
    {
        return WebPStatus::InvalidImage;
    }

    const uint32_t frameCount = WebPDemuxGetI(demux.get(), WEBP_FF_FRAME_COUNT);

    if (frameCount > 1)
    {
        return WebPStatus::AnimatedImagesNotSupported;
    }

    const uint32_t canvasWidth = WebPDemuxGetI(demux.get(), WEBP_FF_CANVAS_WIDTH);
    const uint32_t canvasHeight = WebPDemuxGetI(demux.get(), WEBP_FF_CANVAS_HEIGHT);

    if (canvasWidth == 0 || canvasWidth > static_cast<uint32_t>(std::numeric_limits<int>::max()) ||
        canvasHeight == 0 || canvasHeight > static_cast<uint32_t>(std::numeric_limits<int>::max()))
    {
        return WebPStatus::DecodeFailed;
    }

    WebPStatus status = WebPStatus::Ok;

    WebPIterator iter{};
    if (WebPDemuxGetFrame(demux.get(), 1, &iter))
    {
        size_t outDataSize = 0;
        int outStride = 0;

        void* outData = createImageCallback(
            static_cast<int>(canvasWidth),
            static_cast<int>(canvasHeight),
            outDataSize,
            outStride);

        if (outData)
        {
            status = DecodeImage(
                iter.fragment,
                static_cast<int>(canvasWidth),
                static_cast<int>(canvasHeight),
                outData,
                outDataSize,
                outStride);
        }
        else
        {
            status = WebPStatus::CreateImageCallbackFailed;
        }

        WebPDemuxReleaseIterator(&iter);
    }
    else
    {
        status = WebPStatus::DecodeFailed;
    }

    if (status == WebPStatus::Ok)
    {
        if (!WebPGetImageMetadata(demux.get(), setMetadataCallback))
        {
            status = WebPStatus::SetMetadataCallbackFailed;
        }
    }

    return status;
}

static bool HasTransparency(const void* data, int width, int height, int stride)
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

static int ProgressReport(int percent, const WebPPicture* picture)
{
    ProgressFn callback = reinterpret_cast<ProgressFn>(picture->user_data);
    bool continueProcessing = callback(percent);

    return continueProcessing ? 1 : 0;
}

static WebPStatus EncodeImageMetadata(
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

WebPStatus __stdcall WebPSave(
    const WriteImageFn writeImageCallback,
    const void* bitmap,
    const int width,
    const int height,
    const int stride,
    const EncoderOptions* encodeOptions,
    const EncoderMetadata* metadata,
    ProgressFn callback)
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

    if (callback != nullptr)
    {
        pic->user_data = callback;
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
