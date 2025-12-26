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

#include "WebPDecoder.h"
#include "mux_types.h"
#include "demux.h"
#include "decode.h"
#include "scoped.h"

namespace
{
    bool SetDecoderMetadata(const WebPDemuxer* dmux, SetDecoderMetadataFn setMetadata, MetadataType type)
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

    bool GetImageMetadata(const WebPDemuxer* demux, SetDecoderMetadataFn setMetadata)
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

    WebPStatus DecodeImage(
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
}

WebPStatus __stdcall WebPDecoder::Decode(
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
        if (!GetImageMetadata(demux.get(), setMetadataCallback))
        {
            status = WebPStatus::SetMetadataCallbackFailed;
        }
    }

    return status;
}
