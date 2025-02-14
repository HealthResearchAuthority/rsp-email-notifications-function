using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notify.Client;
using Rsp.NotifyFunction.Contracts;

namespace Rsp.NotifyFunction.Client
{
    public class RSPNotifyClient : IRSPNotifyClient
    {
        private readonly string? _apiKey;
        private readonly ILogger<RSPNotifyClient> _logger;

        public RSPNotifyClient(IConfiguration configuration, ILogger<RSPNotifyClient> logger)
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