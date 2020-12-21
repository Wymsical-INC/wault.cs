using System;

namespace Wault.CS
{
    public class WaultConfigs
    {
        public string ApiUrl { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string DeviceId { get; set; }

        public string AuthorizationEndpoint => BuildUrl(Authority, "/connect/authorize");
        public string TokenEndpoint => BuildUrl(Authority, "/connect/token");
        public string UserInformationEndpoint => BuildUrl(Authority, "/connect/userinfo");
        public string LogoUrl => BuildUrl(Authority, "/images/wym_logo.svg");
        public string SignOutEndpoint => BuildUrl(Authority, "/Account/Logout");

        private static string BuildUrl(string baseUrl, string relativeUrl)
        {
            var baseUri = new Uri(baseUrl);
            var uri = new Uri(baseUri, relativeUrl);
            return uri.ToString();
        }
    }
}
