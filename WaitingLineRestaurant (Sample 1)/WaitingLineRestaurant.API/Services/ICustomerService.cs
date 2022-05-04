using System.Collections.Generic;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Models;
using WaitingLineRestaurant.Infrastructure.Entities;

namespace WaitingLineRestaurant.API.Services
{
    public interface ICustomerService
    {
        Task<IList<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> CreateAsync(CreateCustomer createCustomer);
        Task UpdatePositionAsync(int position, Customer customer);
        Task DeleteAsync(int id);
        Task CallNextAsync(string phone);
        Task<bool> QueueIsActiveAsync(string phone);
        Task RefreshQueueAsync();
    }
}