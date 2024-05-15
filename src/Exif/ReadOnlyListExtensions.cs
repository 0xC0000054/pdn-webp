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

using PaintDotNet;
using PaintDotNet.Collections;
using System.Collections.Generic;

namespace WebPFileType.Exif
{
    internal static class ReadOnlyListExtensions
    {
        internal static T[] AsArrayOrToArray<T>(this IReadOnlyList<T> items)
        {
            if (items is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(items));
            }

            T[]? asArray = items as T[];

            if (asArray is not null)
            {
                return asArray;
            }
            else
            {
                return items.ToArrayEx();
            }
        }
    }
}
