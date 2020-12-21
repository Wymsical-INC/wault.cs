using System;

namespace Wault.CS.Exceptions
{
    public class DiscoveryDocumentFailedException : Exception
    {
        public string AuthorizationServer { get; }

        public DiscoveryDocumentFailedException(string authorizationServer) 
            : base($"Failed to get discovery document from authentication server: {authorizationServer}")
        {
            AuthorizationServer = authorizationServer;
        }
    }
}
