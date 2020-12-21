using System.Threading;
using System.Threading.Tasks;

namespace Wault.CS.Webhook
{
    public interface IWebhookProcessor
    {
        Task ProcessWebhookNotificationAsync(WebhookPayload payload, CancellationToken cancellationToken = default);
    }
}
