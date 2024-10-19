using GroceryShoppingApp.Models;

namespace GroceryShoppingApp.Interfaces
{
    public interface IProductService
    {
        // Definitions for implementations of actions to create, read, update, delete (CRUD)

        Task<IEnumerable<ProductDto>> ListProducts();
        Task<ProductDto?> FindProduct(int id);
        Task<ServiceResponse> UpdateProduct(ProductDto productDto);
        Task<ServiceResponse> AddProduct(ProductDto productDto);
        Task<ServiceResponse> DeleteProduct(int id);

        // Related methods
        Task<IEnumerable<ProductDto>> GetProductsByCategory(string category);

        // Add search method definition
        Task<IEnumerable<ProductDto>> SearchProducts(string query); // New search method
    }
}

