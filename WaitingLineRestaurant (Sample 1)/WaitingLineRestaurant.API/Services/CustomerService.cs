using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Extensions;
using WaitingLineRestaurant.API.Models.Request;
using WaitingLineRestaurant.Infrastructure.Entities;
using WaitingLineRestaurant.Infrastructure.Repositories;

namespace WaitingLineRestaurant.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly IMemoryCache _cache;

        public CustomerService(
            ICustomerRepository customerRepository,
            INotificationService notificationService,
            IMemoryCache cache)
        {
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _cache = cache;
        }

        public async Task<Customer> CreateAsync(CreateCustomerRequest createCustomer)
        {
            var allCustomers = await _customerRepository
                .GetAllAsync();

            var position = allCustomers.Max(m => m.Position) + 1;

            var customer = new Customer
            {
                Name = createCustomer.Name.Trim(),
                Phone = createCustomer.Phone.Trim(),
                Position = position
            };

            await _customerRepository.CreateAsync(customer);

            return customer;
        }

        public async Task DeleteAsync(int id)
        {
            _cache.Remove(Constants.KEY_CACHE_NEXT_CUSTOMER);

            await _customerRepository.DeleteAsync(id);
        }

        public async Task<IList<Customer>> GetAllAsync()
            => await _customerRepository.GetAllAsync();

        public async Task<Customer> GetByIdAsync(int id)
            => await _customerRepository.GetByIdAsync(id);

        public async Task UpdatePositionAsync(int position, Customer customer)
        {
            customer.Position = position;
            await _customerRepository.UpdateAsync(customer.Id, customer);
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
            var allCustomers = await _customerRepository
                .GetAllAsync();

            foreach (var customer in allCustomers)
            {
                var positionUpdated = customer.Position - 1;

                await UpdatePositionAsync(positionUpdated, customer);

                if (_cache.TryGetValue(customer.Phone, out Guid clientId))
                    await _notificationService.SendUpdatePositionEventAsync(
                        clientId, positionUpdated.ToString());
            }
        }

        public async Task<bool> QueueIsActiveAsync(string phone)
        {
            var allCustomers = await _customerRepository
                .GetAllAsync();

            return allCustomers.Any(a => a.Phone == phone);
        }

        public bool CustomerIsNext(string phoneSearch)
            => _cache.TryGetValue(Constants.KEY_CACHE_NEXT_CUSTOMER, out string phoneCache)
            && phoneSearch == phoneCache;

    }
}