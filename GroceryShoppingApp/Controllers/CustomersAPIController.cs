using GroceryShoppingApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GroceryShoppingApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GroceryShoppingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method lists customers
        /// </summary>
        /// <returns>
        /// [{Customer},{Customer}]
        /// </returns>
        /// <example> [{"customerFname":"Narmin","customerLname":"Gurbanli",
        /// {"customerId":2,"customerFname":"Sevinj","customerLname":"Gamarli",}
        /// + number of carts the customer has 
        /// </example>
        [HttpGet(template: "ListCustomers")]
        public List<CustomerDto> ListCustomers()
        {
            List<Customer> Customers = _context.Customers.Include(c=>c.Carts).ToList();
            List<CustomerDto> CustomerDtos = new List<CustomerDto>();
            foreach (Customer Customer in Customers)
            {
                //putting the information from the database into
                //a defined package
                CustomerDtos.Add(new CustomerDto()
                {
                    CustomerLname = Customer.CustomerLname,
                    CustomerFname = Customer.CustomerFname,
                    CustomerAddress = Customer.CustomerAddress,
                    CustomerEmail = Customer.CustomerEmail,
                    CustomerPhone = Customer.CustomerPhone,        
                    CustomerNum = "This customer has " + Customer.Carts.Count + " cart(s)"                    

                });
            }

            return CustomerDtos;


        }


        /// <summary>
        /// Finding customer through their unique id
        /// </summary>
        /// <param name="id">CustomerId, for example /3</param>
        /// <returns>
        /// Returns customers's info, if found, if not - shows error
        /// </returns>
        /// <example>
        /// /api/CustomersAPI/FindCustomer/1
        /// SHows the info of Narmin Gurbanli
        /// </exmaple>

        [HttpGet(template: "FindCustomer/{id}")]
        public IActionResult Find(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer); 
        }


        /// <summary>
        /// Creating new customer
        /// </summary>
        /// <param name="customerDto">The transfer object that outputs info</param>
        /// <returns>
        /// Creates a new customer, or returns an error if the customer input was invalid
        /// </returns>
        /// <example>
        /// /api/CustomersAPI/CreateCustomer       
        /// { 
        ///     "customerFname": "Idayat",
        ///     "customerLname": "Sanni",
        ///     "customerAddress": "123 Humber blv.",
        ///     "customerEmail": "test3@outlook.com",
        ///     "customerPhone": "+1(234)5670000"
        /// }
        /// </example>
        [HttpPost(template: "CreateCustomer")]
        public IActionResult Create([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest("Customer data is null.");
            }

            var customer = new Customer
            {
                CustomerFname = customerDto.CustomerFname,
                CustomerLname = customerDto.CustomerLname,
                CustomerAddress = customerDto.CustomerAddress,
                CustomerEmail = customerDto.CustomerEmail,
                CustomerPhone = customerDto.CustomerPhone

            };

            _context.Customers.Add(customer);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Find), new { id = customer.CustomerId }, customer);
        }


        /// <summary>
        /// Updating an existing customer
        /// </summary>
        /// <param name="id">the {id} of the client that you want to update, for example /1 </param>
        /// <param name="customerDto">The object that outputs info</param>
        /// <returns>
        /// No content if its successful, or an error if its not
        /// </returns>
        /// <example> 
        /// /api/CustomersAPI/UpdateCustomer/1
        /// address update
        /// {
        ///     "customerFname": "Narmin",
        ///     "customerLname": "Gurbanli",
        ///     "customerAddress": "569 Sheppard ave.",
        ///     "customerEmail": "test@outlook.com",
        ///     "customerPhone": "+1(234)5678990"
        /// }
        /// 
        /// </example>
        [HttpPut(template: "UpdateCustomer/{id}")]
        public IActionResult Update(int id, [FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest("Customer data is null.");
            }

            var customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            
            customer.CustomerFname = customerDto.CustomerFname;
            customer.CustomerLname = customerDto.CustomerLname;
            customer.CustomerAddress = customerDto.CustomerAddress;
            customer.CustomerEmail = customerDto.CustomerEmail;
            customer.CustomerPhone = customerDto.CustomerPhone;
            

            _context.Customers.Update(customer);
            _context.SaveChanges();

            return NoContent(); 
        }



        /// <summary>
        /// Deleting a client
        /// </summary>
        /// <param name="id">The {id} of a customer that we want to delete</param>
        /// <returns>
        /// return no content if success and error if the customer was not found
        /// </returns>
        /// <example> 
        /// /api/CustomersAPI/DeleteCustomer/1
        /// doesn not return anything
        /// </example>

        [HttpDelete(template: "DeleteCustomer/{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return NoContent(); 
        }


    }
}


        






 
