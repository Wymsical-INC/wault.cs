using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Wault.CS.Constants;
using Wault.CS.Contracts;
using Wault.CS.Exceptions;
using Wault.CS.Models;

namespace Wault.CS.Services
{
    public class WaultApiClient : IWaultApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly WaultConfigs _options;

        public WaultApiClient(HttpClient httpClient, IOptions<WaultConfigs> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            _options.ApiUrl = _options.ApiUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? _options.ApiUrl : $"{_options.ApiUrl}/";

            _httpClient = httpClient;
        }

        public async Task AddPackageDocumentAsync(Guid packageId, Guid[] documentIds, CancellationToken cancellationToken = default)
        {
            var url = $"api/packages/{packageId}/items";
            var data = new
            {
                itemIds = documentIds
            };

            await PostJsonAsync(url, data, cancellationToken).ConfigureAwait(false);
        }

        public async Task<DocumentCreateResult> CreateDocumentAsync(byte[] fileBytes, string fileName, string name, string[] signatureAccessTokens, DocumentCreationResultType resultType = DocumentCreationResultType.Id, CancellationToken cancellationToken = default)
        {
            await AuthorizeAsync(cancellationToken).ConfigureAwait(false);

            var uriBuilder = new UriBuilder($"{_options.ApiUrl}api/documents");

            using var nameContent = new StringContent(name);
            using var resultTypeContent = new StringContent(nameof(resultType));
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            using var content = new MultipartFormDataContent
            {
                { nameContent, "name" },
                { fileContent, "files", fileName },
                { resultTypeContent, "resultType" }
            };

            if (signatureAccessTokens != null)
            {
                signatureAccessTokens = signatureAccessTokens.Where(p => !string.IsNullOrEmpty(p)).ToArray();

                if (signatureAccessTokens?.Length > 0)
                {
                    var signatureAccessTokensContent = new StringContent(string.Join(",", signatureAccessTokens));
                    content.Add(signatureAccessTokensContent, "signatureAccessTokens");
                }
            }

            try
            {
                using var response = await _httpClient.PostAsync(uriBuilder.Uri, content, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                return JsonSerializer.Deserialize<DocumentCreateResult>(responseString);
            }
            catch (Exception ex)
            {
                throw new WaultRequestFailedException(uriBuilder.Uri.AbsoluteUri, innerException: ex);
            }
        }

        public async Task<PackageCreateResult> CreatePackageAsync(string name, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                Name = name,
                Path = "/"
            };

            return await PostJsonAsync<PackageCreateResult>("api/packages", data, cancellationToken).ConfigureAwait(false);
        }

        public async Task<ClaimModel> GetClaimByAccessToken(string accessToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return new ClaimModel { Value = string.Empty };
            }

            return await GetJsonAsync<ClaimModel>(
                    "api/v2/entries/claim",
                    new Dictionary<string, object> {
                        { "accessToken", accessToken }
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<string> GetSignatureImageBase64StringAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new UriBuilder(_options.ApiUrl + "api/v2/signatures/image");

            if (!string.IsNullOrEmpty(accessToken))
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add("accessToken", accessToken);
                uriBuilder.Query = query.ToString() ?? string.Empty;
            }

            var url = uriBuilder.Uri;
            var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var contentType = response.Content.Headers.ContentType.MediaType;
            var contentBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            var contentString = Convert.ToBase64String(contentBytes);

            return $"data:{contentType};base64, {contentString}";
        }

        public async Task<IEnumerable<WaultModel>> GetWaultListAsync(string entityId, bool businessOnly = true, string role = "CompanyAdmin", bool? requireActive = null, CancellationToken cancellationToken = default)
        {
            var url = $"api/people/{entityId}/agents";
            var queryParameters = new Dictionary<string, object>
            {
                { "companyOnly", businessOnly },
                { "role", role }
            };

            if (requireActive.HasValue)
            {
                queryParameters.Add("requireActive", requireActive.Value);
            }

            return await GetJsonAsync<IEnumerable<WaultModel>>(url, queryParameters, cancellationToken).ConfigureAwait(false);
        }

        public async Task<EntryRequestResult> RequestAccess(Guid[] documentIds, CancellationToken cancellationToken = default)
        {
            return await PostJsonAsync<EntryRequestResult>("api/v2/entries/accessRequests", new { documentIds }, cancellationToken).ConfigureAwait(false);
        }

        public async Task RequestSignaturesAsync(Guid entryId, string[] emails, string comment = null, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                Comment = comment,
                Emails = string.Join(";", emails.Distinct())
            };

            var url = $"api/entries/{entryId}/signatures/request";
            await PostJsonAsync(url, data, cancellationToken).ConfigureAwait(false);
        }

        public async Task SharePackageAsync(Guid packageId, string[] emails, CancellationToken cancellationToken = default)
        {
            if (emails == null)
            {
                throw new ArgumentNullException(nameof(emails));
            }

            var url = $"api/packages/{packageId}/share";

            foreach (var email in emails)
            {
                var data = new Dictionary<string, object>
                {
                    {"shareTo" , email}
                };

                await PostJsonAsync(url, data, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task SignAsync(Guid entryId, string comment = null, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                Comment = comment
            };

            var url = $"api/entries/{entryId}/sign";
            await PostJsonAsync(url, data, cancellationToken).ConfigureAwait(false);
        }

        private async Task AuthorizeAsync(CancellationToken cancellationToken = default)
        {
            var disco = await _httpClient
                .GetDiscoveryDocumentAsync(_options.Authority, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (disco.IsError)
            {
                throw new DiscoveryDocumentFailedException(_options.Authority);
            }

            using var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Scope = "memberships identityserver identityapi coreapi filestorage notifications pointsapi plaidImportApi"
            };

            tokenRequest.Parameters.Add("deviceId", _options.DeviceId);

            var tokenResponse = await _httpClient
                .RequestClientCredentialsTokenAsync(tokenRequest, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (tokenResponse.IsError)
            {
                throw new TokenFailedException(tokenResponse.Error);
            }

            var accessToken = tokenResponse.AccessToken;

            _httpClient.SetBearerToken(accessToken);

            if (!_httpClient.DefaultRequestHeaders.Contains("deviceId"))
            {
                _httpClient.DefaultRequestHeaders.Add("deviceId", _options.DeviceId);
            }
        }

        private async Task<T> GetJsonAsync<T>(string url, IDictionary<string, object> queryParameters, CancellationToken cancellationToken = default)
        {
            await AuthorizeAsync(cancellationToken).ConfigureAwait(false);

            var uriBuilder = new UriBuilder(_options.ApiUrl + url);
            if (queryParameters?.Count > 0)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var queryParameter in queryParameters)
                {
                    query.Add(queryParameter.Key, queryParameter.Value.ToString());
                }

                uriBuilder.Query = query.ToString() ?? string.Empty;
            }

            var responseString = await _httpClient
                .GetStringAsync(uriBuilder.Uri, cancellationToken)
                .ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private async Task<T> PostJsonAsync<T>(string url, object data, CancellationToken cancellationToken = default)
        {
            var response = await PostJsonAsync(url, data, cancellationToken).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(responseString);
        }

        private async Task<HttpResponseMessage> PostJsonAsync(string url, object data, CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                data = new { };
            }

            await AuthorizeAsync(cancellationToken).ConfigureAwait(false);

            var uri = new Uri(_options.ApiUrl + url);
            using var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
