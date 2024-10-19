using GroceryShoppingApp.Data;
using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GroceryShoppingApp.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of database context
        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all carts
        public async Task<IEnumerable<CartDto>> ListCarts()
        {
            List<Cart> carts = await _context.Carts.Include(c => c.Products).ToListAsync();
            List<CartDto> cartDtos = new List<CartDto>();

            foreach (Cart cart in carts)
            {
                cartDtos.Add(new CartDto()
                {
                    CartId = cart.CartId,
                    CartName = cart.CartName,
                    DateCreated = cart.DateCreated
                    
                });
            }

            return cartDtos;
        }

        // Find a cart by ID
        public async Task<CartDto?> FindCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.Customer) // Ensure customer info is included
                .Include(c => c.Products) // Ensure products are included
                .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null)
            {
                return null;
            }

            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                CartName = cart.CartName,
                DateCreated = cart.DateCreated,
                CartCustomer = $"{cart.Customer.CustomerFname} {cart.Customer.CustomerLname}",
                Products = cart.Products?.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,                    
                    Price = p.Price
                }).ToList() ?? new List<ProductDto>() // Ensure Products list is not null
            };

            return cartDto;
        }

        // Add a new cart
        public async Task<ServiceResponse> AddCart(CartDto cartDto)
        {
            ServiceResponse serviceResponse = new();

            Cart cart = new Cart()
            {
                CartName = cartDto.CartName,
                DateCreated = DateTime.UtcNow 
            };

            try
            {
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
                serviceResponse.CreatedId = cart.CartId;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error adding the cart.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Update a cart
        public async Task<ServiceResponse> UpdateCart(CartDto cartDto)
        {
            ServiceResponse serviceResponse = new();

            var cart = await _context.Carts.FindAsync(cartDto.CartId);
            if (cart == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Cart not found.");
                return serviceResponse;
            }

            cart.CartName = cartDto.CartName;

            try
            {
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error updating the cart.");
            }
            catch (Exception ex) 
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add($"Unexpected error: {ex.Message}");
            }

            return serviceResponse;
        }

        // Delete a cart
        public async Task<ServiceResponse> DeleteCart(int id)
        {
            ServiceResponse serviceResponse = new();

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Cart not found.");
                return serviceResponse;
            }

            try
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error deleting the cart.");
            }

            return serviceResponse;
        }

        public async Task<IEnumerable<ProductDto>> ListCartsProducts(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Products) 
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart == null)
            {
                return null; 
            }

            var productDtos = cart.Products.Select(product => new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDesc = product.ProductDesc,
                Category = product.Category,
                Price = product.Price
                
            });

            return productDtos.ToList(); 
        }


        // Add product to a cart
        public async Task<ServiceResponse> AddProductToCart(int cartId, int productId)
        {
            ServiceResponse serviceResponse = new();

            var cart = await _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.CartId == cartId);
            var product = await _context.Products.FindAsync(productId);

            if (cart == null || product == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (cart == null) serviceResponse.Messages.Add("Cart not found.");
                if (product == null) serviceResponse.Messages.Add("Product not found.");
                return serviceResponse;
            }

            try
            {
                cart.Products.Add(product);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error adding product to the cart.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }

        // Remove product from a cart
        public async Task<ServiceResponse> RemoveProductFromCart(int cartId, int productId)
        {
            ServiceResponse serviceResponse = new();

            var cart = await _context.Carts.Include(c => c.Products).FirstOrDefaultAsync(c => c.CartId == cartId);
            var product = await _context.Products.FindAsync(productId);

            if (cart == null || product == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (cart == null) serviceResponse.Messages.Add("Cart not found.");
                if (product == null) serviceResponse.Messages.Add("Product not found.");
                return serviceResponse;
            }

            try
            {
                cart.Products.Remove(product);
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("Error removing product from the cart.");
                serviceResponse.Messages.Add(ex.Message);
            }

            return serviceResponse;
        }




        public async Task<CartDto> GetCartById(int id)
        {
            
            var cart = await _context.Carts
                                     .Include(c => c.Customer) 
                                     .Include(c => c.Products) 
                                     .FirstOrDefaultAsync(c => c.CartId == id);

            if (cart == null) return null;

            
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                CartName = cart.CartName,
                DateCreated = cart.DateCreated,
                CartCustomer = $"{cart.Customer.CustomerFname} {cart.Customer.CustomerLname}", 
                Products = cart.Products.Select(p => new ProductDto
                {
                    ProductName = p.ProductName
                    
                }).ToList()
            };

            return cartDto;
        }

    }
    }

