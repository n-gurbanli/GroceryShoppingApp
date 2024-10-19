using GroceryShoppingApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GroceryShoppingApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Services;

namespace CoreEntityFramework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Returns a list of Customers
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{CustomerDto},{CustomerDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Customer/List -> [{CustomerDto},{CustomerDto},..]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> ListCustomers()
        {
            IEnumerable<CustomerDto> customerDtos = await _customerService.ListCustomers();
            return Ok(customerDtos);
        }

        /// <summary>
        /// Returns a single Customer specified by its {id}
        /// </summary>
        /// <param name="id">The customer id</param>
        /// <returns>
        /// 200 OK
        /// {CustomerDto}
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// GET: api/Customer/Find/1 -> {CustomerDto}
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<CustomerDto>> FindCustomer(int id)
        {
            var customer = await _customerService.FindCustomer(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        /// <summary>
        /// Updates a Customer
        /// </summary>
        /// <param name="id">The ID of the customer to update</param>
        /// <param name="customerDto">The required information to update the customer</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        /// <example>
        /// PUT: api/Customer/Update/5
        /// Request Headers: Content-Type: application/json
        /// Request Body: {CustomerDto}
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, CustomerDto CustomerDto)
        {
           
            if (id != CustomerDto.CustomerId)
            {
                
                return BadRequest();
            }

            ServiceResponse response = await _customerService.UpdateCustomer(CustomerDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

           
            return NoContent();

        }

        /// <summary>
        /// Adds a new Customer
        /// </summary>
        /// <param name="customerDto">The required information to add a customer</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Customer/Find/{CustomerId}
        /// {CustomerDto}
        /// or
        /// 400 Bad Request
        /// </returns>
        /// <example>
        /// POST: api/Customer/Add
        /// Request Headers: Content-Type: application/json
        /// Request Body: {CustomerDto}
        /// -> Response Code: 201 Created
        /// Response Headers: Location: api/Customer/Find/{CustomerId}
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<CustomerDto>> AddCustomer(CustomerDto customerDto)
        {
            ServiceResponse response = await _customerService.AddCustomer(customerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/Customer/Find/{response.CreatedId}", customerDto);
        }

        /// <summary>
        /// Deletes a Customer
        /// </summary>
        /// <param name="id">The id of the customer to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Customer/Delete/7
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            ServiceResponse response = await _customerService.DeleteCustomer(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Returns a list of carts for a specific customer by its {id}
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{CartDto},{CartDto},..]
        /// </returns>
        /// <example>
        /// GET: api/Customer/ListCarts/3 -> [{CartDto},{CartDto},..]
        /// </example>
        [HttpGet(template: "ListCarts/{id}")]
        public async Task<IActionResult> ListCartsForCustomer(int id)
        {
            IEnumerable<CartDto> cartDtos = await _customerService.ListCustomersCarts(id);
            return Ok(cartDtos);
        }

        /// <summary>
        /// Unlinks a cart from a customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <param name="cartId">The id of the cart</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// DELETE: api/Customer/UnlinkCart?CustomerId=4&CartId=12
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpDelete("UnlinkCart")]
        public async Task<ActionResult> UnlinkCart(int customerId, int cartId)
        {
            ServiceResponse response = await _customerService.UnlinkCartFromCustomer(customerId, cartId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Links a cart to a customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        /// <param name="cartId">The id of the cart</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        /// <example>
        /// POST: api/Customer/LinkCart?CustomerId=4&CartId=12
        /// -> Response Code: 204 No Content
        /// </example>
        [HttpPost("LinkCart")]
        public async Task<ActionResult> LinkCart(int customerId, int cartId)
        {
            ServiceResponse response = await _customerService.LinkCartToCustomer(customerId, cartId);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }


    }
}


        






 
