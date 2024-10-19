using GroceryShoppingApp.Models;
using GroceryShoppingApp.Services;

namespace GroceryShoppingApp.Interfaces;

public interface ICartService
{
    // definitions for implementations of actions to create, read, update, delete

    // base CRUD

    Task<IEnumerable<CartDto>> ListCarts();


    Task<CartDto?> FindCart(int id);


    Task<ServiceResponse> UpdateCart(CartDto cartDto);

    Task<ServiceResponse> AddCart(CartDto cartDto);

    Task<ServiceResponse> DeleteCart(int id);

    // related methods

    Task<IEnumerable<ProductDto>> ListCartsProducts(int id);

    


    Task<ServiceResponse> AddProductToCart(int cartId, int productId);

    Task<ServiceResponse> RemoveProductFromCart(int cartId, int productId);
}
