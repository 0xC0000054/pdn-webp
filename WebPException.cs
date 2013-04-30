using System;

namespace WebPFileType
{
    public sealed class WebPException : FormatException
    {
        public WebPException() : base()
        {
        }

        public WebPException(string message)  : base(message)
        {
        }

    }
}
