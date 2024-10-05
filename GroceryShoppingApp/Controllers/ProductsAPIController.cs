using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GroceryShoppingApp.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GroceryShoppingApp.Data;


namespace GroceryShoppingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method lists products
        /// </summary>
        /// <returns>
        /// [{Products},{Product}]
        /// </returns>
        /// <example> 
        ///   {
        ///   "ProductName": "Apple",
        ///   "ProductDesc": "A fresh red apple.",
        ///   "Category": "Fruits",
        ///   "Price": "0.99",
        ///   "Cart": "This product is in Cgherry Pie cart"
        ///   }
        /// </example>
        [HttpGet(template: "ListProducts")]
        public ActionResult<List<ProductDto>> ListProducts()
        {
            // Fetching the products from the database
            List<Product> products = _context.Products.Include(p => p.Carts).ToList();
            List<ProductDto> productDtos = new List<ProductDto>();

            foreach (Product product in products)
            {
                
                string cartNames = string.Join(", ", product.Carts.Select(c => c.CartName));
                productDtos.Add(new ProductDto()
                {
                    ProductName = product.ProductName,
                    ProductDesc = product.ProductDesc,
                    Category = product.Category,
                    Price = product.Price.ToString(),
                    Cart = !string.IsNullOrEmpty(cartNames) ? "This product is in " + cartNames : "This product is not in any cart"
                });
            }

            return productDtos;
        }

        /// <summary>
        /// Finding a product through its unique id
        /// </summary>
        /// <param name="id">ProductId, for example /1</param>
        /// <returns>
        /// Returns product info if found, otherwise shows an error
        /// </returns>
        /// <example>
        /// /api/ProductsAPI/Find/1
        /// </example>
        [HttpGet(template: "Find/{id}")]
        public ActionResult<ProductDto> Find(int id)
        {
            var product = _context.Products.Include(p => p.Carts).FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            string cartNames = string.Join(", ", product.Carts.Select(c => c.CartName));
            var productDto = new ProductDto()
            {
                ProductName = product.ProductName,
                ProductDesc = product.ProductDesc,
                Category = product.Category,
                Price = product.Price.ToString(),
                Cart = !string.IsNullOrEmpty(cartNames) ? "This product is in " + cartNames : "This product is not in any cart"
            };

            return Ok(productDto);
        }

        /// <summary>
        /// Creating a new product
        /// </summary>
        /// <param name="productDto">The transfer object that outputs product info</param>
        /// <returns>
        /// Creates a new product, or returns an error if the input is invalid
        /// </returns>
        /// <example>
        /// /api/ProductsAPI/Create
        /// {
        ///     "ProductName": "Banana",
        ///     "ProductDesc": "A ripe banana.",
        ///     "Category": "Fruits",
        ///     "Price": "0.50"
        /// }
        /// </example>
        [HttpPost(template: "Create")]
        public IActionResult Create([FromBody] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var product = new Product()
            {
                ProductName = productDto.ProductName,
                ProductDesc = productDto.ProductDesc,
                Category = productDto.Category,
                Price = decimal.Parse(productDto.Price) //converting decimal
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Find), new { id = product.ProductId }, productDto);
        }

        /// <summary>
        /// Updating an existing product
        /// </summary>
        /// <param name="id">ProductId</param>
        /// <param name="productDto">Updated product info</param>
        /// <returns>
        /// Returns the updated product or an error if the product is not found
        /// </returns>
        /// <example>
        /// /api/ProductsAPI/Update/1
        /// {
        ///     "ProductName": "Updated Apple",
        ///     "ProductDesc": "A fresh green apple.",
        ///     "Category": "Fruits",
        ///     "Price": "1.00"
        /// }
        /// </example>
        [HttpPut(template: "Update/{id}")]
        public IActionResult Update(int id, [FromBody] ProductDto productDto)
        {
            if (productDto == null)
            {
                return BadRequest("Product data is null.");
            }

            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductName = productDto.ProductName;
            existingProduct.ProductDesc = productDto.ProductDesc;
            existingProduct.Category = productDto.Category;
            existingProduct.Price = decimal.Parse(productDto.Price);

            _context.Products.Update(existingProduct);
            _context.SaveChanges();

            return NoContent(); // 204 No Content
        }

        /// <summary>
        /// Deleting a product
        /// </summary>
        /// <param name="id">ProductId</param>
        /// <returns>
        /// Returns a success message if deleted, or an error if the product is not found
        /// </returns>
        /// <example>
        /// /api/ProductsAPI/Delete/1
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok(); // Return success message
        }
    }
    }
