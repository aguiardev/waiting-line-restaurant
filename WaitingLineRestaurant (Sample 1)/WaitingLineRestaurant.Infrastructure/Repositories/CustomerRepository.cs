using Microsoft.EntityFrameworkCore;
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

        public async Task Create(Customer customer)
        {
            _waitingLineRestaurantContext.Add(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var customer = await GetById(id);

            _waitingLineRestaurantContext.Customers.Remove(customer);
            await _waitingLineRestaurantContext.SaveChangesAsync();
        }

        public async Task<IList<Customer>> GetAll()
            => await _waitingLineRestaurantContext.Customers.ToListAsync();

        public async Task<Customer> GetById(int id)
            => await _waitingLineRestaurantContext.Customers.FirstOrDefaultAsync(f => f.Id == id);

        public async Task Update(int id, Customer customer)
        {
            var currentCustomer = await GetById(id);

            currentCustomer.Name = customer.Name;
            currentCustomer.PeopleQuantity = customer.PeopleQuantity;
            currentCustomer.Phone = customer.Phone;
            currentCustomer.Position = customer.Position;

            await _waitingLineRestaurantContext.SaveChangesAsync();
        }
    }
}
