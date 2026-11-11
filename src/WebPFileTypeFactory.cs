////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet.FileTypes;

namespace WebPFileType
{
    public sealed class WebPFileTypeFactory :
        IFileTypeFactory
    {
        public IFileType[] CreateFileTypes(IFileTypeHost host)
        {
            return [new WebPFileType(host)];
        }
    }
}
