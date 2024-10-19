using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Models;
using Microsoft.EntityFrameworkCore;
using GroceryShoppingApp.Data;
using GroceryShoppingApp.Services;
using System;

namespace GroceryShoppingApp.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        
        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all customers
        public async Task<IEnumerable<CustomerDto>> ListCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            List<CustomerDto> customerDtos = new List<CustomerDto>();

            foreach (Customer customer in customers)
            {
                customerDtos.Add(new CustomerDto()
                {
                    CustomerId = customer.CustomerId,
                    CustomerFname = customer.CustomerFname,
                    CustomerLname = customer.CustomerLname,
                    CustomerAddress = customer.CustomerAddress,
                    CustomerEmail = customer.CustomerEmail,
                    CustomerPhone = customer.CustomerPhone,
                    CustomerNum = "Customer has " + customer.Carts?.Count() + " cart(s)"
                });
            }

            return customerDtos;
        }

        // Find a customer by ID
        public async Task<CustomerDto?> FindCustomer(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto()
            {
                CustomerId = customer.CustomerId,
                CustomerFname = customer.CustomerFname,
                CustomerLname = customer.CustomerLname,
                CustomerAddress = customer.CustomerAddress,
                CustomerEmail = customer.CustomerEmail,
                CustomerPhone = customer.CustomerPhone,
                CustomerNum = "Customer has " + customer.Carts?.Count() + " cart(s)"
            };
        }

        // Add a new customer
        public async Task<ServiceResponse> AddCustomer(CustomerDto customerDto)
        {
            ServiceResponse serviceResponse = new();

            Customer customer = new Customer()
            {
                CustomerFname = customerDto.CustomerFname,
                CustomerLname = customerDto.CustomerLname,
                CustomerAddress = customerDto.CustomerAddress,
                CustomerEmail = customerDto.CustomerEmail,
                CustomerPhone = customerDto.CustomerPhone
            };

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = customer.CustomerId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error adding the customer.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Update a customer
        public async Task<ServiceResponse> UpdateCustomer(CustomerDto customerDto)
        {
            ServiceResponse serviceResponse = new();

            Customer customer = new Customer()
            {
                CustomerId = customerDto.CustomerId,
                CustomerFname = customerDto.CustomerFname,
                CustomerLname = customerDto.CustomerLname,
                CustomerAddress = customerDto.CustomerAddress,
                CustomerEmail = customerDto.CustomerEmail,
                CustomerPhone = customerDto.CustomerPhone
            };

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error updating the customer.");
            }

            return serviceResponse;
        }

        // Delete a customer
        public async Task<ServiceResponse> DeleteCustomer(int id)
        {
            ServiceResponse serviceResponse = new();

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Customer not found.");
                return serviceResponse;
            }

            try
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error deleting the customer.");
            }

            return serviceResponse;
        }



        //List Carts of Customers

        public async Task<IEnumerable<CartDto>> ListCustomersCarts(int customerId)
        {
            var carts = await _context.Carts
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Products) 
                .ToListAsync();

            return carts.Select(c => new CartDto
            {
                CartId = c.CartId,
                CartName = c.CartName,
                DateCreated = c.DateCreated,
                Products = c.Products.Select(p => new ProductDto 
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    
                }).ToList()
            });
        }



        // Link a customer to a cart
        public async Task<ServiceResponse> LinkCartToCustomer(int customerId, int cartId)
        {
            ServiceResponse serviceResponse = new();

            var customer = await _context.Customers
                .Include(c => c.Carts)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            var cart = await _context.Carts.FindAsync(cartId);

            if (customer == null || cart == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (customer == null)
                {
                    serviceResponse.Messages.Add("Customer not found.");
                }
                if (cart == null)
                {
                    serviceResponse.Messages.Add("Cart not found.");
                }
                return serviceResponse;
            }

            try
            {
                customer.Carts.Add(cart);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error linking the cart to the customer.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Unlink a customer from a cart
        public async Task<ServiceResponse> UnlinkCartFromCustomer(int customerId, int cartId)
        {
            ServiceResponse serviceResponse = new();

            var customer = await _context.Customers
                .Include(c => c.Carts)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            var cart = await _context.Carts.FindAsync(cartId);

            if (customer == null || cart == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (customer == null)
                {
                    serviceResponse.Messages.Add("Customer not found.");
                }
                if (cart == null)
                {
                    serviceResponse.Messages.Add("Cart not found.");
                }
                return serviceResponse;
            }

            try
            {
                customer.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error unlinking the cart from the customer.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync(); 
        }

    }
}

