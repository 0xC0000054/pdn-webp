using System;

namespace WebPFileType
{
    [Serializable]
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
