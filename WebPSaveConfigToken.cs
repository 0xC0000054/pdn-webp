////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-webp, a FileType plugin for Paint.NET
// that loads and saves WebP images.
//
// Copyright (c) 2011-2018 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using System;

namespace WebPFileType
{
    [Serializable]
    public sealed class WebPSaveConfigToken : PaintDotNet.SaveConfigToken
    {
        public WebPPreset Preset
        {
            get;
            internal set;
        }

        public int Quality
        {
            get;
            internal set;
        }

        public int Method
        {
            get;
            internal set;
        }

        public int NoiseShaping
        {
            get;
            internal set;
        }

        public int FilterStrength
        {
            get;
            internal set;
        }

        public int Sharpness
        {
            get;
            internal set;
        }

        public WebPFilterType FilterType
        {
            get;
            internal set;
        }

        public int FileSize
        {
            get;
            internal set;
        }

        public bool EncodeMetaData
        {
            get;
            internal set;
        }

        public WebPSaveConfigToken(
            WebPPreset preset,
            int quality,
            int method,
            int noiseShaping,
            int filterStrength,
            int sharpness,
            WebPFilterType filterType,
            int fileSize,
            bool encodeMetaData)
        {
            Preset = preset;
            Quality = quality;
            Method = method;
            NoiseShaping = noiseShaping;
            FilterStrength = filterStrength;
            Sharpness = sharpness;
            FilterType = filterType;
            FileSize = fileSize;
            EncodeMetaData = encodeMetaData;
        }

        private WebPSaveConfigToken(WebPSaveConfigToken copyMe)
        {
            Preset = copyMe.Preset;
            Quality = copyMe.Quality;
            Method = copyMe.Method;
            NoiseShaping = copyMe.NoiseShaping;
            FilterStrength = copyMe.FilterStrength;
            Sharpness = copyMe.Sharpness;
            FilterType = copyMe.FilterType;
            FileSize = copyMe.FileSize;
            EncodeMetaData = copyMe.EncodeMetaData;
        }

        public override object Clone()
        {
            return new WebPSaveConfigToken(this);
        }
    }
}
