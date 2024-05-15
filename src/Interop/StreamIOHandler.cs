////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2024 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace WebPFileType.Interop
{
    internal sealed class StreamIOHandler
    {
        private readonly Stream output;

        public StreamIOHandler(Stream output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public Exception? WriteException { get; private set; }

        public WebPStatus WriteImageCallback(IntPtr image, UIntPtr imageSize)
        {
            if (image == IntPtr.Zero)
            {
                return WebPStatus.InvalidParameter;
            }

            if (imageSize == UIntPtr.Zero)
            {
                // Ignore zero-length images.
                return WebPStatus.Ok;
            }

            // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
            const int MaxBufferSize = 81920;

            try
            {
                long size = checked((long)imageSize.ToUInt64());

                int bufferSize = (int)Math.Min(size, MaxBufferSize);

                byte[] streamBuffer = new byte[bufferSize];

                output.SetLength(size);

                long offset = 0;
                long remaining = size;

                while (remaining > 0)
                {
                    int copySize = (int)Math.Min(MaxBufferSize, remaining);

                    Marshal.Copy(new IntPtr(image.ToInt64() + offset), streamBuffer, 0, copySize);

                    output.Write(streamBuffer, 0, copySize);

                    offset += copySize;
                    remaining -= copySize;
                }
            }
            catch (OperationCanceledException)
            {
                return WebPStatus.UserAbort;
            }
            catch (Exception ex)
            {
                WriteException = ex;
                return WebPStatus.BadWrite;
            }

            return WebPStatus.Ok;
        }
    }
}
