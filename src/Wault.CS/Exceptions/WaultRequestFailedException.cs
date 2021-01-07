using System;

namespace Wault.CS.Exceptions
{
    public class WaultRequestFailedException : Exception
    {
        public WaultRequestFailedException(string requestUrl, object requestObject = null, Exception innerException = null)
            : base("Request Wault API failed", innerException)
        {
            RequestUrl = requestUrl;
            RequestObject = requestObject;
        }

        public object RequestObject { get; set; }
        public string RequestUrl { get; set; }
    }
}
