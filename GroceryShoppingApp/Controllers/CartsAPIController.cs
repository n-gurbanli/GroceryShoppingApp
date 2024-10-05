using GroceryShoppingApp.Data;
using GroceryShoppingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace GroceryShoppingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns carts list
        /// </summary>
        /// <returns>
        /// Returns list of carts with additional information, like products in cart and customer info
        /// </returns>
        /// <example> 
        /// /api/CartsAPI/ListCarts
        ///         "cartName": "Cherry Pie",
        ///         "dateCreated": "2024-09-30T12:00:00Z",
        ///         "productsInCart": "Products: Cherries, Honey",
        ///         "cartCustomer": "This cart belongs to Narmin Gurbanli"
        /// </example>
        [HttpGet(template: "ListCarts")]
        public List<CartDto> ListCarts()
        {
            List<Cart> carts = _context.Carts
                .Include(c => c.Products)
                .Include(c => c.Customer)
                .ToList();

            List<CartDto> cartDtos = new List<CartDto>();

            foreach (Cart cart in carts)
            {
                
                string productList = string.Join(", ", cart.Products.Select(p => p.ProductName));

                // Putting the information from the database into a defined package
                cartDtos.Add(new CartDto()
                {
                    CartName = cart.CartName,
                    DateCreated = cart.DateCreated,
                    ProductsInCart = "Products: " + productList,  // Listing product names
                    CartCustomer = "This cart belongs to " + cart.Customer.CustomerFname + " " + cart.Customer.CustomerLname // Space between names
                });
            }

            return cartDtos;
        }

        /// <summary>
        /// Finding carts through their unique id
        /// </summary>
        /// <param name="id">CartId, for example /3</param>
        /// <returns>
        /// Returns cart's info, if found, if not - shows error
        /// </returns>
        /// <example>
        /// /api/CartsAPI/FindCart/1
        /// SHows the info of "Apple Salad Cart"
        /// </exmaple>
        [HttpGet(template: "FindCart/{id}")]
        public IActionResult Find(int id)
        {
                var cart = _context.Carts
                .Include(c => c.Customer)   
                .Include(c => c.Products)   
                .FirstOrDefault(c => c.CartId == id);  

            if (cart == null)
            {
                return NotFound();
            }

            
            var cartDto = new CartDto
            {
                CartName = cart.CartName,
                DateCreated = cart.DateCreated,
                CartCustomer = "This cart belongs to " + cart.Customer.CustomerFname + " " + cart.Customer.CustomerLname,
                ProductsInCart = "Products: " + string.Join(", ", cart.Products.Select(p => p.ProductName))
            };

            return Ok(cartDto);
        }

        /// <summary>
        /// Creating new cart
        /// </summary>
        /// <param name="cartDto">The transfer object that outputs info</param>
        /// <returns>
        /// Creates a new cart, or returns an error if the cart input was invalid
        /// </returns>
        /// <example>
        /// /api/CartsAPI/AddCart      
        /// { 
        ///         "cartName": "Chicken Soup",
        ///         "dateCreated": "2024-10-30T12:00:00Z",
        ///         "productsInCart": "Products: Chicken, Onions, Carrots, Noodles",
        ///         "cartCustomer": "This cart belongs to Idayat Sanni"
        /// }
        /// </example>
        [HttpPost(template: "AddCart")]
        public async Task<ActionResult<Cart>> AddCart(CartDto cartDto)
        {
            
            var cart = new Cart
            {
                CartName = cartDto.CartName,
                DateCreated = DateTime.UtcNow, 
                //I didn't quite understand if i should update my linked  databases from here, so I didnt add two ther parametrs
                
            };

            // Add the new cart to the database
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            // Returns 201 Created with the location of the created cart
            return CreatedAtAction(nameof(ListCarts), new { id = cart.CartId }, cartDto);
        }

        /// <summary>
        /// Updating an existing cart by its {id}
        /// </summary>
        /// <param name="id">The {id} of the cart that you want to update, for example /1</param>
        /// <param name="cartDto">The transfer object that contains the updated cart information</param>
        /// <returns>
        /// No content if successful, or an error if the cart input is invalid or not found
        /// </returns>
        /// <example>
        /// /api/CartsAPI/UpdateCart/1
        /// { 
        ///     "cartName": "Chicken Soup",
        ///     "productsInCart": "Products: Chicken, Onions, Carrots, Noodles, Salt",
        ///     "cartCustomer": "This cart belongs to Idayat Sanni"
        /// }
        /// </example>
        [HttpPut(template: "UpdateCart/{id}")]
        public IActionResult UpdateCart(int id, [FromBody] CartDto cartDto)
        {
            if (cartDto == null)
            {
                return BadRequest("Cart data is null.");
            }

            var existingCart = _context.Carts.Include(c => c.Products).Include(c => c.Customer).FirstOrDefault(c => c.CartId == id);

            if (existingCart == null)
            {
                return NotFound("Cart not found.");
            }

            
            existingCart.CartName = cartDto.CartName;

            
            existingCart.Products.Clear();

            if (!string.IsNullOrEmpty(cartDto.ProductsInCart))
            {
                string[] productNames = cartDto.ProductsInCart.Replace("Products: ", "").Split(", ");
                foreach (string productName in productNames)
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductName == productName);
                    if (product != null)
                    {
                        existingCart.Products.Add(product);
                    }
                }
            }

            
            var customerNames = cartDto.CartCustomer.Split(' ');
            if (customerNames.Length >= 2)
            {
                existingCart.Customer.CustomerFname = customerNames[0];
                existingCart.Customer.CustomerLname = customerNames[1];
            }

            
            _context.SaveChanges();

            return NoContent();
        }


        /// <summary>
        /// Deleting a cart by its {id}
        /// </summary>
        /// <param name="id">The {id} of the cart that you want to delete, for example /1</param>
        /// <returns>
        /// No content if successful, or an error if the cart was not found
        /// </returns>
        /// <example>
        /// /api/CartsAPI/DeleteCart/1
        /// </example>
        [HttpDelete(template: "DeleteCart/{id}")]
        public IActionResult DeleteCart(int id)
        {
            
            var cart = _context.Carts.Include(c => c.Products).Include(c => c.Customer).FirstOrDefault(c => c.CartId == id);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            
            _context.Carts.Remove(cart);
            _context.SaveChanges();

            return NoContent(); 
        }
    }
    }

