using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using WaitingLineRestaurant.API.Services;

namespace WaitingLineRestaurant.API.Extensions
{
    public static class EndpointsExtensions
    {
        /// <summary>
        /// Registra o endpoint para as implementações de <see cref="ServerSentEventsService"/>
        /// </summary>
        /// <param name="app">Instância do <see cref="ApplicationBuilder"/></param>
        /// <returns><see cref="IApplicationBuilder"/></returns>
        public static IApplicationBuilder UseSSE(this IApplicationBuilder app)
            => app.MapServerSentEvents<NotificationService>(Constants.URL_SSE);
    }
}