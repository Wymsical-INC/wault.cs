using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wault.CS.Webhook
{
    public class WebhookProcessor : IWebhookProcessor
    {
        private readonly IWebhookHandler _webhookHandler;

        public WebhookProcessor(IWebhookHandler webhookHandler)
        {
            _webhookHandler = webhookHandler;
        }

        public async Task ProcessWebhookNotificationAsync(WebhookPayload payload, CancellationToken cancellationToken = default)
        {
            payload = payload ?? throw new ArgumentNullException(nameof(payload));
            switch (payload.Action)
            {
                case WebhookAction.EntryRequestAccepted:
                    await _webhookHandler
                        .EntryRequestAccepted(payload, cancellationToken)
                        .ConfigureAwait(false);
                    break;

                case WebhookAction.EntryRequestRejected:
                    await _webhookHandler
                        .EntryRequestRejected(payload, cancellationToken)
                        .ConfigureAwait(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(payload));
            }
        }
    }
}
