using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Rsp.NotifyFunction.Application.Contracts;

namespace Rsp.NotifyFunction.Client
{
    public class NotifyClient : INotifyClient
    {
        private readonly string? _apiKey;
        private readonly ILogger<NotifyClient> _logger;

        public NotifyClient(IConfiguration configuration, ILogger<NotifyClient> logger)
        {
            _logger = logger;
            _apiKey = configuration["GovNotifyAPIKey"];
        }

        public NotificationClient GetClient()
        {
            NotificationClient client = new NotificationClient(_apiKey);
            return client;
        }
    }
}