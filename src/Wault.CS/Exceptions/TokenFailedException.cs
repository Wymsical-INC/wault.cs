using System;

namespace Wault.CS.Exceptions
{
    public class TokenFailedException : Exception
    {
        public string Error { get; }

        public TokenFailedException(string error)
            : base($"Getting token failed: {error}")
        {
            Error = error;
        }
    }
}
