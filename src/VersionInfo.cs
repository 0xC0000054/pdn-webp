////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2023 Nicholas Hayes
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
        {
            int libwebpVersion = WebPNative.GetLibWebPVersion();

            int major = (libwebpVersion >> 16) & 0xff;
            int minor = (libwebpVersion >> 8) & 0xff;
            int revision = libwebpVersion & 0xff;

            return $"{major}.{minor}.{revision}";
        }

        private static string GetPluginVersion()
            => typeof(VersionInfo).Assembly.GetName().Version.ToString();
    }
}
