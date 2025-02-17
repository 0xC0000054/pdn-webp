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

using PaintDotNet;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace WebPFileType.Interop
{
    internal sealed class DecoderCreateImage
    {
        private Surface? surface;

        public DecoderCreateImage()
        {
            surface = null;
            CallbackErrorInfo = null;
        }

        public ExceptionDispatchInfo? CallbackErrorInfo { get; private set; }

        public unsafe void* CreateImage(int width, int height, out nuint dataSize, out int stride)
        {
            dataSize = 0;
            stride = 0;

            try
            {
                surface = new Surface(width, height);
                stride = surface.Stride;
                dataSize = (nuint)surface.Scan0.Length;

                return surface.Scan0.VoidStar;
            }
            catch (Exception ex)
            {
                CallbackErrorInfo = ExceptionDispatchInfo.Capture(ex);
                return null;
            }
        }

        public Surface? GetSurface() => Interlocked.Exchange(ref surface, null);
    }
}
