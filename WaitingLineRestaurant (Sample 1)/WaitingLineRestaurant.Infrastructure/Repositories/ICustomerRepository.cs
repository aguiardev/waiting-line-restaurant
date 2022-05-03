using System.Collections.Generic;
using System.Threading.Tasks;
using WaitingLineRestaurant.Infrastructure.Entities;

namespace WaitingLineRestaurant.Infrastructure.Repositories
{
    public interface ICustomerRepository
    {
        Task<IList<Customer>> GetAll();
        Task<Customer> GetById(int id);
        Task Create(Customer customer);
        Task Update(int id, Customer customer);
        Task Delete(int id);
    }
}