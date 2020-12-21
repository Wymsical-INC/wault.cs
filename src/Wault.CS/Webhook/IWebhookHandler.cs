using System.Threading;
using System.Threading.Tasks;

namespace Wault.CS.Webhook
{
    public interface IWebhookHandler
    {
        Task EntryRequestAccepted(WebhookPayload payload, CancellationToken cancellationToken = default);

        Task EntryRequestRejected(WebhookPayload payload, CancellationToken cancellationToken = default);

        Task DocumentActivated(WebhookPayload payload, CancellationToken cancellationToken = default);
    }
}
