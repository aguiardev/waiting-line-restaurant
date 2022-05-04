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

                var userPhone = GetUserPhoneFromQueryParam(request.Query);

                if (_cache.TryGetValue(userPhone, out string queueId) || string.IsNullOrEmpty(queueId))
                {
                    _logger.LogError("Queue not identified.");
                    return;
                }

                switch (AddToGroup(queueId, client))
                {
                    case ServerSentEventsAddToGroupResult.AddedToExistingGroup:
                        _logger.LogInformation($"[OnDisconnect] User {userPhone} added to exsting group {queueId}; ConnectionId: {client.Id}");
                        break;
                    case ServerSentEventsAddToGroupResult.AddedToNewGroup:
                        _logger.LogInformation($"[OnDisconnect] User {userPhone} added to new group {queueId}; ConnectionId: {client.Id}");
                        break;
                }
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

                var userPhone = GetUserPhoneFromQueryParam(request.Query);

                _logger.LogInformation($"[OnDisconnect] Connection closed for account code {userPhone}; ConnectionId {client.Id}");
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