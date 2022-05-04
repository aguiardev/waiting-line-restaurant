﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaitingLineRestaurant.Infrastructure.Entities;

namespace WaitingLineRestaurant.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly WaitingLineRestaurantContext _waitingLineRestaurantContext;

        public CustomerRepository(WaitingLineRestaurantContext waitingLineRestaurantContext)
            => _waitingLineRestaurantContext = waitingLineRestaurantContext;

        public async Task CreateAsync(Customer customer)
        {
            _waitingLineRestaurantContext.Add(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);

            _waitingLineRestaurantContext.Customers.Remove(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task<IList<Customer>> GetAllAsync()
            => await _waitingLineRestaurantContext.Customers.ToListAsync();

        public async Task<Customer> GetByIdAsync(int id)
            => await _waitingLineRestaurantContext.Customers.FirstOrDefaultAsync(f => f.Id == id);

        public async Task UpdateAsync(int id, Customer customer)
        {
            var currentCustomer = await GetByIdAsync(id);

            currentCustomer.Name = customer.Name;
            currentCustomer.Phone = customer.Phone;
            currentCustomer.Position = customer.Position;

            await _waitingLineRestaurantContext.SaveChangesAsync();
        }
    }
}
