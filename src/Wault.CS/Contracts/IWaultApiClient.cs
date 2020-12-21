using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wault.CS.Models;

namespace Wault.CS.Contracts
{
    public interface IWaultApiClient
    {
        Task AddPackageDocumentAsync(Guid packageId, Guid[] documentIds, CancellationToken cancellationToken = default);

        Task<DocumentCreateResult> CreateDocumentAsync(byte[] file, string fileName, string name, string[] signatureAccessTokens, CancellationToken cancellationToken = default);

        Task<PackageCreateResult> CreatePackageAsync(string name, CancellationToken cancellationToken = default);

        Task<ClaimModel> GetClaimByAccessToken(string accessToken, CancellationToken cancellationToken = default);

        Task<string> GetSignatureImageBase64StringAsync(string accessToken, CancellationToken cancellationToken = default);

        Task<IEnumerable<WaultModel>> GetWaultListAsync(string entityId, bool businessOnly = true, string role = "CompanyAdmin", bool? requireActive = null, CancellationToken cancellationToken = default);

        Task<EntryRequestResult> RequestAccess(Guid[] documentIds, CancellationToken cancellationToken = default);

        Task RequestSignaturesAsync(Guid entryId, string[] emails, string comment = null, CancellationToken cancellationToken = default);

        Task SharePackageAsync(Guid packageId, string[] emails, CancellationToken cancellationToken = default);

        Task SignAsync(Guid entryId, string comment = null, CancellationToken cancellationToken = default);
    }
}
