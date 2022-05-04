using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Services;

namespace WaitingLineRestaurant.API.Extensions
{
    public static class ServerSentEventExtensions
    {
        public static async Task SendNextEventAsync(
            this INotificationService notificationService, Guid clientId, string message)
        {
            var client = notificationService.GetClient(clientId);

            if (client is null || !client.IsConnected)
                return;

            var sseEvent = PrepareEvent(Constants.MSG_TYPE_NEXT, message);

            await notificationService.SendEventAsync(sseEvent);
        }

        public static async Task SendUpdatePositionEventAsync(
            this INotificationService notificationService, Guid clientId, string message)
        {
            var client = notificationService.GetClient(clientId);

            if (client is null || !client.IsConnected)
                return;

            var sseEvent = PrepareEvent(Constants.MSG_TYPE_UPDATE_POSITION, message);

            await notificationService.SendEventAsync(sseEvent);
        }

        private static ServerSentEvent PrepareEvent(string type, string msg) => new()
        {
            Id = Guid.NewGuid().ToString(),
            Type = type,
            Data = new List<string>()
            {
                msg
            }
        };
    }
}