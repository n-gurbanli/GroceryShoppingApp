using Microsoft.AspNetCore.Mvc;
using GroceryShoppingApp.Interfaces; 
using GroceryShoppingApp.Models;
using System.Threading.Tasks;
using GroceryShoppingApp.ViewModels;

namespace GroceryShoppingApp.Controllers
{
    public class CustomerPageController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerPageController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customers/List
        public async Task<IActionResult> List()
        {
            var customers = await _customerService.ListCustomers();
            return View(customers);
        }

        // GET: Customers/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.FindCustomer(id);
            if (customer == null)
            {
                return NotFound();
            }

            
            var carts = await _customerService.ListCustomersCarts(id);

            
            var viewModel = new CustomerDetailsViewModel
            {
                Customer = customer,
                Carts = carts
            };

            return View(viewModel);
        }

        // GET: Customers/New
        public IActionResult New()
        {
            return View(); 
        }

        // POST: Customers/Add
        [HttpPost]
        public async Task<IActionResult> Add(CustomerDto customerDto)
        {
            

            ServiceResponse response = await _customerService.AddCustomer(customerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "CustomerPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        
    

            // GET: Customers/Edit/{id}
            public async Task<IActionResult> Edit(int id)
                {
                    var customer = await _customerService.FindCustomer(id);
                    if (customer == null)
                    {
                        return NotFound();
                    }
                    return View(customer);
                }

        // POST: Customers/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, CustomerDto customerDto)
        {

            var response = await _customerService.UpdateCustomer(customerDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            
            return RedirectToAction("Details", new { id = customerDto.CustomerId });
        }

        // GET: Customers/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.FindCustomer(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _customerService.DeleteCustomer(id);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }
            return RedirectToAction(nameof(List));
        }

        // GET: Customers/DeleteConfirmation/{id}
        public async Task<IActionResult> DeleteConfirmation(int id)
        {
            var customer = await _customerService.FindCustomer(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }
    }
}

