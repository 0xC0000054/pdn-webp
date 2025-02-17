﻿////////////////////////////////////////////////////////////////////////
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

using System;

namespace WebPFileType
{
    [Serializable]
    public sealed class WebPException : FormatException
    {
        public WebPException() : base()
        {
        }

        public WebPException(string message)  : base(message)
        {
        }
    }
}
