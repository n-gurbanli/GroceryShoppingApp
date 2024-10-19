using GroceryShoppingApp.Data;
using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GroceryShoppingApp.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> ListProducts()
        {
            var products = await _context.Products.ToListAsync();
            return products.Select(product => new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDesc = product.ProductDesc,
                Category = product.Category,
                Price = product.Price
            });
        }

        public async Task<ProductDto> FindProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return null;

            return new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDesc = product.ProductDesc,
                Category = product.Category,
                Price = product.Price
            };
        }

        public async Task<ServiceResponse> AddProduct(ProductDto productDto)
        {
            ServiceResponse serviceResponse = new();

            
            Product product = new Product()
            {
                ProductName = productDto.ProductName,
                ProductDesc = productDto.ProductDesc,
                Category = productDto.Category,
                Price = productDto.Price
            };

            

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Product.");
                serviceResponse.Messages.Add(ex.Message);
                return serviceResponse; 
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = product.ProductId;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateProduct(ProductDto productDto)
        {
            ServiceResponse serviceResponse = new();
            var product = await _context.Products.FindAsync(productDto.ProductId);
            if (product == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Product not found.");
                return serviceResponse;
            }

            product.ProductName = productDto.ProductName;
            product.ProductDesc = productDto.ProductDesc;
            product.Category = productDto.Category;
            product.Price = productDto.Price;

            await _context.SaveChangesAsync();
            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteProduct(int productId)
        {
            ServiceResponse serviceResponse = new();
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Product not found.");
                return serviceResponse;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }


        //products by category

        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category cannot be null or empty", nameof(category));
            }

            
            var products = await _context.Products
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    Category = p.Category,
                    Price = p.Price
                })
                .ToListAsync();

            return products;
        }

        public async Task<IEnumerable<ProductDto>> SearchProducts(string query)
        {
            return await _context.Products
                                 .Where(p => p.ProductName.Contains(query) || p.ProductDesc.Contains(query))
                                 .Select(p => new ProductDto
                                 {
                                     ProductId = p.ProductId,
                                     ProductName = p.ProductName,
                                     ProductDesc = p.ProductDesc,
                                     Category = p.Category,
                                     Price = p.Price,
                                     
                                     Cart = string.Join(", ", p.Carts.Select(c => c.CartName))
                                 })
                                 .ToListAsync();
        }
    }
}
