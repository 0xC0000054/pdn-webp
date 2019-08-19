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
using System.Drawing.Imaging;

namespace WebPFileType.Exif
{
    internal sealed class ExifValueCollection : IEnumerable<PropertyItem>
    {
        private readonly List<PropertyItem> exifMetadata;

        public ExifValueCollection(List<PropertyItem> items)
        {
            exifMetadata = items ?? throw new ArgumentNullException(nameof(items));
        }

        public int Count => exifMetadata.Count;

        public PropertyItem GetAndRemoveValue(ExifTagID tag)
        {
            int tagID = unchecked((ushort)tag);

            PropertyItem value = exifMetadata.Find(p => p.Id == tagID);

            if (value != null)
            {
                exifMetadata.RemoveAll(p => p.Id == tagID);
            }

            return value;
        }

        public IEnumerator<PropertyItem> GetEnumerator()
        {
            return exifMetadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return exifMetadata.GetEnumerator();
        }
    }
}
