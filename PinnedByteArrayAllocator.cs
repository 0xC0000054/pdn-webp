////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2016, 2018 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace WebPFileType
{
    // Adapted from: https://blog.getpaint.net/2012/04/30/marshaling-native-arrays-back-as-managed-arrays-without-copying/

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate IntPtr PinnedByteArrayAllocDelegate(UIntPtr sizeInBytes);

    internal sealed class PinnedByteArrayAllocator : IDisposable
    {
        private Dictionary<IntPtr, byte[]> pinnedArrayLookup;
        private List<GCHandleContainer> pinnedBufferHandles;
        private bool disposed;

        public PinnedByteArrayAllocator()
        {
            pinnedArrayLookup = new Dictionary<IntPtr, byte[]>();
            pinnedBufferHandles = new List<GCHandleContainer>();
            disposed = false;
        }

        public IntPtr AllocateArray(UIntPtr sizeInBytes)
        {
            ulong size = sizeInBytes.ToUInt64();
            byte[] buffer = new byte[size];
            GCHandleContainer handle = new GCHandleContainer(buffer, GCHandleType.Pinned);

            IntPtr pbArray = handle.AddrOfPinnedObject();
            pinnedArrayLookup.Add(pbArray, buffer);
            pinnedBufferHandles.Add(handle);

            return pbArray;
        }

        public byte[] GetManagedArray(IntPtr pbArray)
        {
            return pinnedArrayLookup[pbArray];
        }

        public void Dispose()
        {
            if (!disposed)
            {
                for (int i = 0; i < pinnedBufferHandles.Count; i++)
                {
                    pinnedBufferHandles[i].Dispose();
                }
                pinnedBufferHandles = null;
                pinnedArrayLookup = null;
                disposed = true;
            }
        }

        private sealed class GCHandleContainer : CriticalFinalizerObject, IDisposable
        {
            private GCHandle handle;
            private bool disposed;

            public GCHandleContainer(object value, GCHandleType type)
            {
                handle = GCHandle.Alloc(value, type);
            }

            ~GCHandleContainer()
            {
                Dispose(false);
            }

            public IntPtr AddrOfPinnedObject()
            {
                return handle.AddrOfPinnedObject();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                    }

                    handle.Free();
                    disposed = true;
                }
            }
        }
    }
}
