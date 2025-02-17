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

using PaintDotNet.Imaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebPFileType.Exif
{
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(ExifValueCollectionDebugView))]
    internal sealed class ExifValueCollection : IEnumerable<KeyValuePair<ExifPropertyPath, ExifValue>>
    {
        private readonly Dictionary<ExifPropertyPath, ExifValue> exifMetadata;

        public ExifValueCollection(Dictionary<ExifPropertyPath, ExifValue> items)
        {
            exifMetadata = items ?? throw new ArgumentNullException(nameof(items));
        }

        public int Count => exifMetadata.Count;

        public ExifValue? GetAndRemoveValue(ExifPropertyPath key)
        {
            return exifMetadata.Remove(key, out ExifValue? value) ? value : null;
        }

        public IEnumerator<KeyValuePair<ExifPropertyPath, ExifValue>> GetEnumerator()
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
            public KeyValuePair<ExifPropertyPath, ExifValue>[] Items
            {
                get
                {
                    return collection.exifMetadata.ToArray();
                }
            }
        }
    }
}
