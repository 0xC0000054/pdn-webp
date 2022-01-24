////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2022 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet.Imaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebPFileType.Exif
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ExifValueCollectionDebugView))]
    internal sealed class ExifValueCollection : IEnumerable<ExifPropertyItem>
    {
        private readonly List<ExifPropertyItem> exifMetadata;

        public ExifValueCollection(List<ExifPropertyItem> items)
        {
            exifMetadata = items ?? throw new ArgumentNullException(nameof(items));
        }

        public int Count => exifMetadata.Count;

        public ExifPropertyItem GetAndRemoveValue(ExifPropertyPath key)
        {
            ExifPropertyItem value = exifMetadata.Find(p => p.Path == key);

            if (value != null)
            {
                exifMetadata.RemoveAll(p => p.Path == key);
            }

            return value;
        }

        public IEnumerator<ExifPropertyItem> GetEnumerator()
        {
            return exifMetadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return exifMetadata.GetEnumerator();
        }

        private sealed class ExifValueCollectionDebugView
        {
            private readonly ExifValueCollection collection;

            public ExifValueCollectionDebugView(ExifValueCollection collection)
            {
                this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ExifPropertyItem[] Items
            {
                get
                {
                    return collection.exifMetadata.ToArray();
                }
            }
        }
    }
}
