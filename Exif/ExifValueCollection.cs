////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2019 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebPFileType.Exif
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ExifValueCollectionDebugView))]
    internal sealed class ExifValueCollection : IEnumerable<MetadataEntry>
    {
        private readonly List<MetadataEntry> exifMetadata;

        public ExifValueCollection(List<MetadataEntry> items)
        {
            exifMetadata = items ?? throw new ArgumentNullException(nameof(items));
        }

        public int Count => exifMetadata.Count;

        public MetadataEntry GetAndRemoveValue(MetadataKey key)
        {
            MetadataEntry value = exifMetadata.Find(p => p.Section == key.Section && p.TagId == key.TagId);

            if (value != null)
            {
                exifMetadata.RemoveAll(p => p.Section == key.Section && p.TagId == key.TagId);
            }

            return value;
        }

        public IEnumerator<MetadataEntry> GetEnumerator()
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
            public MetadataEntry[] Items
            {
                get
                {
                    return collection.exifMetadata.ToArray();
                }
            }
        }
    }
}
