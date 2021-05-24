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

#pragma once

#include "decode.h"
#include "encode.h"
#include "mux_types.h"
#include "mux.h"
#include "demux.h"
#include <memory>

struct webp_mux_deleter
{
    void operator()(WebPMux* mux)
    {
        if (mux != nullptr)
        {
            WebPMuxDelete(mux);
        }
    }
};

typedef std::unique_ptr<WebPMux, webp_mux_deleter> ScopedWebPMux;

class ScopedWebPMuxAssembler
{
public:
    ScopedWebPMuxAssembler(WebPMux* mux) : data(new(std::nothrow)WebPData)
    {
        if (data != nullptr)
        {
            status = WebPMuxAssemble(mux, data);
        }
        else
        {
            status = WEBP_MUX_MEMORY_ERROR;
        }
    }

    ~ScopedWebPMuxAssembler()
    {
        Release();
    }

    // Disable copying and assignment.
    ScopedWebPMuxAssembler(const ScopedWebPMuxAssembler&) = delete;
    const ScopedWebPMuxAssembler& operator=(ScopedWebPMuxAssembler&) = delete;

    const uint8_t* GetBuffer() const
    {
        if (data != nullptr)
        {
            return data->bytes;
        }
        else
        {
            return nullptr;
        }
    }

    size_t GetBufferSize() const
    {
        if (data != nullptr)
        {
            return data->size;
        }
        else
        {
            return 0;
        }
    }

    WebPMuxError GetStatus() const
    {
        return status;
    }

    void Release()
    {
        if (data != nullptr)
        {
            WebPDataClear(data);
            delete data;
            data = nullptr;
        }
    }

private:
    WebPData* data;
    WebPMuxError status;
};

struct webp_demux_deleter
{
    void operator()(WebPDemuxer* mux)
    {
        if (mux != nullptr)
        {
            WebPDemuxDelete(mux);
        }
    }
};

typedef std::unique_ptr<WebPDemuxer, webp_demux_deleter> ScopedWebPDemuxer;

class ScopedWebPPicture
{
public:

    ScopedWebPPicture() : picture(new(std::nothrow)WebPPicture), initialized(false)
    {
        if (picture != nullptr)
        {
            initialized = WebPPictureInit(picture) != 0;
        }
    }

    ~ScopedWebPPicture()
    {
        Release();
    }

    // Disable copying and assignment.
    ScopedWebPPicture(const ScopedWebPPicture&) = delete;
    const ScopedWebPPicture& operator=(const ScopedWebPPicture&) = delete;

    WebPPicture* Get() const
    {
        return picture;
    }

    bool IsInitalized() const
    {
        return initialized;
    }

    void Release()
    {
        if (picture != nullptr)
        {
            if (initialized)
            {
                WebPPictureFree(picture);
            }

            delete picture;
            picture = nullptr;
        }
    }

    int operator==(nullptr_t)
    {
        return picture == nullptr;
    }

    WebPPicture* operator->()
    {
        return picture;
    }

private:
    WebPPicture* picture;
    bool initialized;
};

class ScopedWebPMemoryWriter
{
public:

    ScopedWebPMemoryWriter() : writer(new(std::nothrow)WebPMemoryWriter)
    {
        if (writer != nullptr)
        {
            WebPMemoryWriterInit(writer);
        }
    }

    ~ScopedWebPMemoryWriter()
    {
        Release();
    }

    // Disable copying and assignment.
    ScopedWebPMemoryWriter(const ScopedWebPMemoryWriter&) = delete;
    const ScopedWebPMemoryWriter& operator=(const ScopedWebPMemoryWriter&) = delete;

    WebPMemoryWriter* Get() const
    {
        return writer;
    }

    uint8_t* GetBuffer() const
    {
        if (writer != nullptr)
        {
            return writer->mem;
        }
        else
        {
            return nullptr;
        }
    }

    size_t GetBufferSize() const
    {
        if (writer != nullptr)
        {
            return writer->size;
        }
        else
        {
            return 0;
        }
    }

    void Release()
    {
        if (writer != nullptr)
        {
            WebPMemoryWriterClear(writer);

            delete writer;
            writer = nullptr;
        }
    }

    int operator==(nullptr_t)
    {
        return writer == nullptr;
    }

private:
    WebPMemoryWriter* writer;
};
