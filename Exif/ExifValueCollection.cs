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

namespace WebPFileType.Exif
{
    internal sealed class ExifValueCollection : IEnumerable<MetadataEntry>
    {
        private readonly List<MetadataEntry> exifMetadata;

        public ExifValueCollection(List<MetadataEntry> items)
        {
            exifMetadata = items ?? throw new ArgumentNullException(nameof(items));
        }

        public int Count => exifMetadata.Count;

        public MetadataEntry GetAndRemoveValue(ExifTagID tag)
        {
            ushort tagID = unchecked((ushort)tag);

            MetadataEntry value = exifMetadata.Find(p => p.TagId == tagID);

            if (value != null)
            {
                exifMetadata.RemoveAll(p => p.TagId == tagID);
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
    }
}
