////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2020 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

#if !PDN_3_5_X
using PaintDotNet.WebP;
using System;
using System.Linq;
using System.Collections.Generic;
using WebPFileType.Properties;

namespace WebPFileType
{
    internal sealed class PdnLocalizedStringResourceManager
        : IWebPStringResourceManager
    {
        private readonly IWebPFileTypeStrings strings;
        private static readonly IReadOnlyDictionary<string, WebPFileTypeStringNames> pdnLocalizedStringMap;

        static PdnLocalizedStringResourceManager()
        {
            // Use a dictionary to map the resource name to its WebPFileTypeStringNames value.
            // This avoids repeated calls to Enum.TryParse.
            // Adapted from https://stackoverflow.com/a/13677446
            pdnLocalizedStringMap = Enum.GetValues(typeof(WebPFileTypeStringNames))
                                        .Cast<WebPFileTypeStringNames>()
                                        .ToDictionary(kv => kv.ToString(), kv => kv, StringComparer.OrdinalIgnoreCase);
        }

        public PdnLocalizedStringResourceManager(IWebPFileTypeStrings strings)
        {
            this.strings = strings;
        }

        public string GetString(string name)
        {
            if (pdnLocalizedStringMap.TryGetValue(name, out WebPFileTypeStringNames value))
            {
                return strings?.TryGetString(value) ?? Resources.ResourceManager.GetString(name);
            }
            else
            {
                return Resources.ResourceManager.GetString(name);
            }
        }
    }
}
#endif
