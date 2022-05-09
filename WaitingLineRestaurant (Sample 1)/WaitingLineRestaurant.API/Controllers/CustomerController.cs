using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaitingLineRestaurant.API.Models.Request;
using WaitingLineRestaurant.API.Services;

namespace WaitingLineRestaurant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
            => _customerService = customerService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var customers = await _customerService.GetAllAsync();

            return customers == null || !customers.Any()
                ? NotFound()
                : Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerRequest createCustomer)
        {
            try
            {
                var customer = await _customerService.CreateAsync(createCustomer);

                if (customer.Id <= 0)
                    return BadRequest();
                
                return CreatedAtAction(
                    "Create", new { id = customer.Id }, createCustomer);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("{phone}/next")]
        public async Task<IActionResult> CallNextAsync(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    return BadRequest("");

                await _customerService.CallNextAsync(phone);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("{phone}/next")]
        public IActionResult CustomerIsNextAsync(string phone)
        {
            try
            {
                return _customerService.CustomerIsNext(phone) ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerService.GetByIdAsync(id);
            if (currentCustomer == null)
                return NotFound();

            await _customerService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPut("queue/refresh")]
        public async Task<IActionResult> RefreshQueueAsync()
        {
            await _customerService.RefreshQueueAsync();

            return Ok();
        }

        [HttpGet("queue/active/{phone}")]
        public async Task<IActionResult> QueueIsActiveAsync(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return BadRequest("Invalid phone");

            var customer = await _customerService.QueueIsActiveAsync(phone);

            return customer ? Ok() : NotFound();
        }
    }
}