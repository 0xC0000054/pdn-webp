////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2021 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using WebPFileType.Properties;

namespace WebPFileType
{
    internal sealed class BuiltinStringResourceManager
        : IWebPStringResourceManager
    {
        public string GetString(string name)
        {
            return Resources.ResourceManager.GetString(name);
        }
    }
}
