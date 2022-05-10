using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Entities;
using WaitingLineRestaurant.API.Extensions;
using WaitingLineRestaurant.API.Models.Request;

namespace WaitingLineRestaurant.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly WaitingLineRestaurantContext _waitingLineRestaurantContext;
        private readonly INotificationService _notificationService;
        private readonly IMemoryCache _cache;

        public CustomerService(
            WaitingLineRestaurantContext waitingLineRestaurantContext,
            INotificationService notificationService,
            IMemoryCache cache)
        {
            _waitingLineRestaurantContext = waitingLineRestaurantContext;
            _notificationService = notificationService;
            _cache = cache;
        }

        public async Task<Customer> CreateAsync(CreateCustomerRequest createCustomer)
        {
            var customer = new Customer
            {
                Name = createCustomer.Name.Trim(),
                Phone = createCustomer.Phone.Trim()
            };

            var allCustomers = await GetAllAsync();

            if (allCustomers.Any())
                customer.Position = allCustomers.Max(m => m.Position) + 1;

            await _waitingLineRestaurantContext.AddAsync(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);

            _cache.Remove(Constants.KEY_CACHE_NEXT_CUSTOMER);

            _waitingLineRestaurantContext.Customers.Remove(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task<IList<Customer>> GetAllAsync()
            => await _waitingLineRestaurantContext.Customers.ToListAsync();

        public async Task<Customer> GetByIdAsync(int id)
            => await _waitingLineRestaurantContext.Customers.FirstOrDefaultAsync(f => f.Id == id);

        public async Task UpdatePositionAsync(int id, int position)
        {
            var currentCustomer = await GetByIdAsync(id);

            currentCustomer.Position = position;

            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task CallNextAsync(string phone)
        {
            if (_cache.TryGetValue(phone, out Guid clientId))
                await _notificationService.SendNextEventAsync(
                    clientId, Constants.MSG_NEXT);

            _cache.Set(Constants.KEY_CACHE_NEXT_CUSTOMER, phone);
        }

        public async Task RefreshQueueAsync()
        {
            var allCustomers = await GetAllAsync();

            foreach (var customer in allCustomers)
            {
                var positionUpdated = customer.Position - 1;

                await UpdatePositionAsync(customer.Id, positionUpdated);

                if (_cache.TryGetValue(customer.Phone, out Guid clientId))
                    await _notificationService.SendUpdatePositionEventAsync(
                        clientId, positionUpdated.ToString());
            }
        }

        public async Task<bool> QueueIsActiveAsync(string phone)
        {
            var allCustomers = await GetAllAsync();

            return allCustomers.Any(a => a.Phone == phone);
        }

        public bool CustomerIsNext(string phoneSearch)
            => _cache.TryGetValue(Constants.KEY_CACHE_NEXT_CUSTOMER, out string phoneCache)
            && phoneSearch == phoneCache;

    }
}