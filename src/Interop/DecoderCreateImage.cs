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

using PaintDotNet.Imaging;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace WebPFileType.Interop
{
    internal sealed class DecoderCreateImage
    {
        private IBitmap<ColorBgra32>? bitmap;
        private IBitmapLock<ColorBgra32>? bitmapLock;

        public DecoderCreateImage()
        {
            bitmap = null;
            CallbackErrorInfo = null;
        }

        public ExceptionDispatchInfo? CallbackErrorInfo { get; private set; }

        public unsafe void* CreateImage(int width, int height, out nuint dataSize, out int stride)
        {
            dataSize = 0;
            stride = 0;

            try
            {
                using IImagingFactory imagingFactory = ImagingFactory.CreateRef();
                bitmap = imagingFactory.CreateBitmap<ColorBgra32>(width, height);
                bitmapLock = bitmap.Lock(BitmapLockOptions.ReadWrite);
                stride = bitmapLock.BufferStride;
                dataSize = (nuint)bitmapLock.BufferSize;

                return bitmapLock.Buffer;
            }
            catch (Exception ex)
            {
                CallbackErrorInfo = ExceptionDispatchInfo.Capture(ex);
                return null;
            }
        }

        public IBitmap<ColorBgra32>? GetBitmap()
        {
            IBitmap<ColorBgra32>? bitmap = Interlocked.Exchange(ref this.bitmap, null);
            IBitmapLock<ColorBgra32>? bitmapLock = Interlocked.Exchange(ref this.bitmapLock, null);
            bitmapLock?.Dispose();
            return bitmap;
        }
    }
}
