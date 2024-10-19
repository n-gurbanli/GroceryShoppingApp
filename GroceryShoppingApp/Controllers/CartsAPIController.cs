using GroceryShoppingApp.Data;
using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryShoppingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsAPIController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService; 
        private readonly IProductService _productService; 

        public CartsAPIController(ICartService cartService, ICustomerService customerService) 
        {
            _cartService = cartService;
            _customerService = customerService; 
        }

        /// <summary>
        /// Returns carts list
        /// </summary>
        /// <returns>Returns list of carts with additional information.</returns>
        /// <example>/api/CartsAPI/ListCarts</example>
        [HttpGet("ListCarts")]
        public async Task<ActionResult<IEnumerable<CartDto>>> ListCarts()
        {
            var cartDtos = await _cartService.ListCarts();
            return Ok(cartDtos);
        }

        /// <summary>
        /// Finding carts through their unique id
        /// </summary>
        /// <param name="id">CartId, for example /3</param>
        /// <returns>Returns cart's info, if found, otherwise shows error.</returns>
        /// <example>/api/CartsAPI/FindCart/1</example>
        [HttpGet("FindCart/{id}")]
        public async Task<IActionResult> Find(int id)
        {
            var cartDto = await _cartService.FindCart(id);
            if (cartDto == null)
            {
                return NotFound("Cart not found.");
            }
            return Ok(cartDto);
        }

        /// <summary>
        /// Creating new cart
        /// </summary>
        /// <param name="cartDto">The transfer object that outputs info</param>
        /// <returns>Creates a new cart, or returns an error if the cart input was invalid.</returns>
        /// <example>/api/CartsAPI/AddCart</example>
        [HttpPost("AddCart")]
        public async Task<ActionResult> AddCart(CartDto cartDto)
        {
            var serviceResponse = await _cartService.AddCart(cartDto);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.Created)
            {
                return CreatedAtAction(nameof(Find), new { id = serviceResponse.CreatedId }, cartDto);
            }
            return BadRequest(serviceResponse.Messages);
        }

        /// <summary>
        /// Updating a cart by its {id}
        /// </summary>
        /// <param name="id">The {id} of the cart that you want to update, for example /1</param>
        /// <param name="cartDto">The transfer object that contains the updated cart information</param>
        /// <returns>No content if successful, or an error if the cart input is invalid or not found.</returns>
        /// <example>/api/CartsAPI/UpdateCart/1</example>
        [HttpPut("UpdateCart/{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] CartDto cartDto)
        {
            cartDto.CartId = id; 
            var serviceResponse = await _cartService.UpdateCart(cartDto);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            return NoContent();
        }

        /// <summary>
        /// Deleting a cart by its {id}
        /// </summary>
        /// <param name="id">The {id} of the cart that you want to delete, for example /1</param>
        /// <returns>No content if successful, or an error if the cart was not found.</returns>
        /// <example>/api/CartsAPI/DeleteCart/1</example>
        [HttpDelete("DeleteCart/{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var serviceResponse = await _cartService.DeleteCart(id);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            return NoContent();
        }

        // Get products in the cart to show them in the view
        /// <summary>
        /// Retrieves the list of products associated with a specific shopping cart.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart.</param>
        /// <returns>
        /// An ActionResult containing:
        /// - 200 OK with a list of ProductDto objects if the products are found.
        /// - 404 Not Found if the cart does not exist.
        /// </returns>
        /// <example>
        /// GET: /api/CartsAPI/{cartId}/products
        /// </example>
        [HttpGet("{cartId}/products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetCartProducts(int cartId)
        {
            var productDtos = await _cartService.ListCartsProducts(cartId);
            if (productDtos == null)
            {
                return NotFound("Cart not found.");
            }
            return Ok(productDtos);
        }

        // Add product to cart (Build a view in the future)
        /// <summary>
        /// Adds a specified product to a shopping cart.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart to which the product will be added.</param>
        /// <param name="productId">The unique identifier of the product to be added.</param>
        /// <returns>
        /// An ActionResult containing:
        /// - 200 OK with a ServiceResponse if the product is successfully added to the cart.
        /// - 404 Not Found if the cart or product does not exist.
        /// - 400 Bad Request if there was an error adding the product, with details in the response messages.
        /// </returns>
        /// <example>
        /// POST: /api/CartsAPI/{cartId}/products/{productId}
        /// </example>
        [HttpPost("{cartId}/products/{productId}")]
        public async Task<ActionResult<ServiceResponse>> AddProductToCart(int cartId, int productId)
        {
            var serviceResponse = await _cartService.AddProductToCart(cartId, productId);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(serviceResponse.Messages);
            }
            return Ok(serviceResponse);
        }

        // Remove product from cart (Build a view in the future)
        /// <summary>
        /// Removes a specified product from a shopping cart.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart from which the product will be removed.</param>
        /// <param name="productId">The unique identifier of the product to be removed.</param>
        /// <returns>
        /// 
        /// - 200 OK with a ServiceResponse if the product is successfully removed from the cart
        /// - 404 Not Found if the cart or product does not exist
        /// - 400 Bad Request if there was an error removing the products
        /// </returns>
        /// <example>
        /// DELETE: /api/CartsAPI/{cartId}/products/{productId}
        /// </example>

        [HttpDelete("{cartId}/products/{productId}")]
        public async Task<ActionResult<ServiceResponse>> RemoveProductFromCart(int cartId, int productId)
        {
            var serviceResponse = await _cartService.RemoveProductFromCart(cartId, productId);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.Error)
            {
                return BadRequest(serviceResponse.Messages);
            }
            return Ok(serviceResponse);
        }

        
    }
}
