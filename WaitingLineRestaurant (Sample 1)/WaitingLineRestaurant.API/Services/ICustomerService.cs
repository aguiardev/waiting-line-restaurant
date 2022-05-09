using System.Collections.Generic;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Models.Request;
using WaitingLineRestaurant.API.Entities;

namespace WaitingLineRestaurant.API.Services
{
    public interface ICustomerService
    {
        Task<IList<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> CreateAsync(CreateCustomerRequest createCustomer);
        Task UpdatePositionAsync(int id, int position);
        Task DeleteAsync(int id);
        Task CallNextAsync(string phone);
        Task<bool> QueueIsActiveAsync(string phone);
        bool CustomerIsNext(string phoneSearch);
        Task RefreshQueueAsync();
    }
}