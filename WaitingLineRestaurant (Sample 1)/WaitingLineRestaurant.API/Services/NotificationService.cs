using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace WaitingLineRestaurant.API.Services
{
    public class NotificationService : ServerSentEventsService, INotificationService
    {
        private readonly ILogger<INotificationService> _logger;
        private readonly IMemoryCache _cache;

        public NotificationService(
            IOptions<ServerSentEventsServiceOptions<NotificationService>> options,
            ILogger<NotificationService> logger,
            IMemoryCache cache)
            : base(options.ToBaseServerSentEventsServiceOptions())
        {
            _logger = logger;
            _cache = cache;
        }

        public override async Task OnConnectAsync(HttpRequest request, IServerSentEventsClient client)
        {
            try
            {
                await base.OnConnectAsync(request, client);

                var customerPhone = GetUserPhoneFromQueryParam(request.Query);

                var clientId = await _cache.GetOrCreateAsync(customerPhone, entry =>
                {
                    return Task.FromResult(client.Id);
                });

                //if (!_cache.TryGetValue(customerPhone, out Guid clientId))
                //{
                //    _cache.Set(customerPhone, client.Id);
                //}

                _logger.LogInformation($"[OnConnect] User {customerPhone} is connected; ConnectionId: {clientId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public override async Task OnDisconnectAsync(HttpRequest request, IServerSentEventsClient client)
        {
            try
            {
                await base.OnDisconnectAsync(request, client);

                var customerPhone = GetUserPhoneFromQueryParam(request.Query);

                _cache.Remove(customerPhone);

                _logger.LogInformation($"[OnDisconnect] Connection closed for account code {customerPhone}; ConnectionId {client.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private static string GetUserPhoneFromQueryParam(IQueryCollection query)
        {
            if (!query.TryGetValue(Constants.QUERY_PARAM_PHONE, out var userPhone))
                return null;

            return userPhone;
        }
    }
}