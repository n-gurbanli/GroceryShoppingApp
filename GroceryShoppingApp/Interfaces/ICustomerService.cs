using GroceryShoppingApp.Models;
using GroceryShoppingApp.Services;

namespace GroceryShoppingApp.Interfaces
{
    public interface ICustomerService
    {
        // Base CRUD methods
        Task<IEnumerable<CustomerDto>> ListCustomers();
        Task<CustomerDto?> FindCustomer(int id);
        Task<ServiceResponse> UpdateCustomer(CustomerDto customerDto);
        Task<ServiceResponse> AddCustomer(CustomerDto customerDto);
        Task<ServiceResponse> DeleteCustomer(int id);

        // Related methods
        Task<IEnumerable<CartDto>> ListCustomersCarts(int id);
        Task<ServiceResponse> LinkCartToCustomer(int customerId, int cartId);
        Task<ServiceResponse> UnlinkCartFromCustomer(int customerId, int cartId);
    }
}


