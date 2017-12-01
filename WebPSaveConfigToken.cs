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
            this.Preset = preset;
            this.Quality = quality;
            this.Method = method;
            this.NoiseShaping = noiseShaping;
            this.FilterStrength = filterStrength;
            this.Sharpness = sharpness;
            this.FilterType = filterType;
            this.FileSize = fileSize;
            this.EncodeMetaData = encodeMetaData;
        }

        private WebPSaveConfigToken(WebPSaveConfigToken copyMe)
        {
            this.Preset = copyMe.Preset;
            this.Quality = copyMe.Quality;
            this.Method = copyMe.Method;
            this.NoiseShaping = copyMe.NoiseShaping;
            this.FilterStrength = copyMe.FilterStrength;
            this.Sharpness = copyMe.Sharpness;
            this.FilterType = copyMe.FilterType;
            this.FileSize = copyMe.FileSize;
            this.EncodeMetaData = copyMe.EncodeMetaData;
        }

        public override object Clone()
        {
            return new WebPSaveConfigToken(this);
        }

    }
}
