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
            try
            {
                var customers = await _customerService.GetAllAsync();

                return customers == null || !customers.Any()
                    ? NotFound()
                    : Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{phone}/next")]
        public async Task<IActionResult> CallNextAsync(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    return BadRequest("Customer phone is empty");

                await _customerService.CallNextAsync(phone);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{phone}/next")]
        public IActionResult CustomerIsNextAsync(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    return BadRequest("Customer phone is empty");

                return _customerService.CustomerIsNext(phone) ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid customer id");

                var currentCustomer = await _customerService.GetByIdAsync(id);
                if (currentCustomer == null)
                    return NotFound();

                await _customerService.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("queue/refresh")]
        public async Task<IActionResult> RefreshQueueAsync()
        {
            try
            {
                await _customerService.RefreshQueueAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("queue/active/{phone}")]
        public async Task<IActionResult> QueueIsActiveAsync(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    return BadRequest("Customer phone is empty");

                var customer = await _customerService.QueueIsActiveAsync(phone);

                return customer ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}