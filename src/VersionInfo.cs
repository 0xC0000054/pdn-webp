////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2026 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;

namespace WebPFileType
{
    internal static class VersionInfo
    {
        private static readonly Lazy<string> libwebpVersion = new(GetLibWebPVersion);
        private static readonly Lazy<string> pluginVersion = new(GetPluginVersion);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static string LibWebPVersion => libwebpVersion.Value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static string PluginVersion => pluginVersion.Value;

        private static string GetLibWebPVersion()
            => WebPNative.GetLibWebPVersion().ToString();

        private static string GetPluginVersion()
            => typeof(VersionInfo).Assembly.GetName().Version!.ToString();
    }
}
