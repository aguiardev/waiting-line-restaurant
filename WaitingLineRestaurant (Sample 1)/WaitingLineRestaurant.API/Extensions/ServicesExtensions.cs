using Lib.AspNetCore.ServerSentEvents;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WaitingLineRestaurant.API.Services;
using WaitingLineRestaurant.Infrastructure;
using WaitingLineRestaurant.Infrastructure.Repositories;

namespace WaitingLineRestaurant.API.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string connectionStringSqlite)
        {
            if (string.IsNullOrEmpty(connectionStringSqlite))
                throw new ArgumentException("Sqlite connection string is null!");

            services.AddDbContext<WaitingLineRestaurantContext>(opt =>
                opt.UseSqlite(connectionStringSqlite));

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            
            AddSSE(services);

            return services;
        }

        private static void AddSSE(this IServiceCollection services)
        {
            services.AddServerSentEvents();
            services.AddResponseCompression(_ => _.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "text/event-stream" }));
            services.AddServerSentEvents<INotificationService, NotificationService>(options =>
            {
                options.KeepaliveMode = ServerSentEventsKeepaliveMode.Always;
                options.KeepaliveInterval = 30;
                options.ReconnectInterval = 5000;
            });
        }
    }
}
