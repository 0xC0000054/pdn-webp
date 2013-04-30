// -----------------------------------------------------------------------
// <copyright file="WebPSaveConfigToken.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebPFileType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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

        public int Method
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

        public WebPSaveConfigToken(WebPPreset preset, int quality, int filterStrength, int sharpness, WebPFilterType filterType, int method, int fileSize, bool encodeMetaData)
        {
            this.Preset = preset;
            this.Quality = quality;
            this.FilterStrength = filterStrength;
            this.Sharpness = sharpness;
            this.FilterType = filterType;
            this.Method = method;
            this.FileSize = fileSize;
            this.EncodeMetaData = encodeMetaData;
        }

        private WebPSaveConfigToken(WebPSaveConfigToken copyMe) 
        {
            this.Preset = copyMe.Preset;
            this.Quality = copyMe.Quality;
            this.FilterStrength = copyMe.FilterStrength;
            this.Sharpness = copyMe.Sharpness;
            this.FilterType = copyMe.FilterType;
            this.Method = copyMe.Method;
            this.FileSize = copyMe.FileSize;
            this.EncodeMetaData = copyMe.EncodeMetaData;
        }

        public override object Clone()
        {
            return new WebPSaveConfigToken(this);
        }

    }
}
