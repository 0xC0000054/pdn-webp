using System;
using System.Collections.Generic;
using PaintDotNet;
using System.Reflection;

namespace WebPFileType
{
    class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get 
            {
                return "null54";
            }
        }

        public string Copyright
        {
            get 
            {
                return ((AssemblyCopyrightAttribute)(typeof(PluginSupportInfo).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0])).Copyright;
            }
        }

        public string DisplayName
        {
            get 
            {
                return "WebP FileType";
            }
        }

        public Version Version
        {
            get 
            {
                return typeof(PluginSupportInfo).Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }
}
