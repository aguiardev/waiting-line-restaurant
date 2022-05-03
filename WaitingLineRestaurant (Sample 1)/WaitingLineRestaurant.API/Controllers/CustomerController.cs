using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WaitingLineRestaurant.Infrastructure.Entities;
using WaitingLineRestaurant.Infrastructure.Repositories;

namespace WaitingLineRestaurant.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            await _customerRepository.Create(customer);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
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
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerRepository.GetById(id);
            if (currentCustomer == null)
                return NotFound();

            await _customerRepository.Delete(id);

            return NoContent();
        }
    }
}
