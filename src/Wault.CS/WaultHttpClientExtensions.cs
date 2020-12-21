using System;
using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Extensions.Http;

using Wault.CS.Contracts;
using Wault.CS.Services;

namespace Wault.CS
{
    public static class WaultHttpClientExtensions
    {
        public static IHttpClientBuilder AddWaultSDK(this IServiceCollection services)
        {
            return services
                .AddHttpClient<IWaultApiClient, WaultApiClient>("WaultHttpClient")
                .AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
