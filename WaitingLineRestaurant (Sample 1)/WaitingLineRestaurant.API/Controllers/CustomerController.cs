using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using WaitingLineRestaurant.Infrastructure.Entities;
using WaitingLineRestaurant.Infrastructure.Repositories;

namespace WaitingLineRestaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
            => _customerRepository = customerRepository;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAll();

            return customers == null || !customers.Any()
                ? NotFound()
                : Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid customer id");

            var customer = await _customerRepository.GetById(id);

            return customer == null
                ? NotFound()
                : Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] Customer customer,
            [FromServices] IMemoryCache cache)
        {
            try
            {
                await _customerRepository.Create(customer);

                if (customer.Id > 0)
                    return BadRequest();

                cache.Set(customer.Phone, customer.PeopleQuantity);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = customer.Id },
                    customer
                );
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            if (id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerRepository.GetById(id);
            if (currentCustomer == null)
                return NotFound();

            await _customerRepository.Update(id, customer);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromServices] IMemoryCache cache)
        {
            if (id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerRepository.GetById(id);
            if (currentCustomer == null)
                return NotFound();

            await _customerRepository.Delete(id);
            cache.Remove(currentCustomer.Phone);

            return NoContent();
        }
    }
}
