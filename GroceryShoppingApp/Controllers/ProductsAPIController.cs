using GroceryShoppingApp.Interfaces;
using GroceryShoppingApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace GroceryShoppingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsAPIController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Returns the list of products.
        /// </summary>
        /// <returns>A list of ProductDto.</returns>
        /// /// <example>
        /// GET: /api/ProductAPI/ListProducts
        /// </example>
        [HttpGet("ListProducts")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> ListProducts()
        {
            var productDtos = await _productService.ListProducts();
            return Ok(productDtos);
        }

        /// <summary>
        /// Finds a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// /// <example>
        /// GET: /api/ProductAPI/5
        /// </example>
        [HttpGet("{id}")]
        public async Task<IActionResult> FindProduct(int id)
        {
            var productDto = await _productService.FindProduct(id);
            if (productDto == null)
            {
                return NotFound("Product not found.");
            }
            return Ok(productDto);
        }

        /// <summary>
        /// Adds a Product
        /// </summary>
        /// <param name="ProductDto">The required information to add the Product (ProductName, ProductDesc, Category, Price)</param>
        /// 
        /// <example>
        /// POST: api/Product/Add
        /// </example>
        [HttpPost("Add")] // Use route attribute to define the endpoint
        public async Task<ActionResult<Product>> AddProduct([FromBody] ProductDto productDto) 
        {
            ServiceResponse response = await _productService.AddProduct(productDto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            
            return Created($"api/Product/FindProduct/{response.CreatedId}", productDto); 
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// <param name="productDto">Updated product data.</param>
        /// <example>
        /// PUT: /api/ProductAPI/5 (body returns updated product)
        /// </example>
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            productDto.ProductId = id; 
            var serviceResponse = await _productService.UpdateProduct(productDto);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            return NoContent();
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">Product ID.</param>
        /// /// <example>
        /// DELETE: /api/ProductAPI/Delete/5
        /// </example>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var serviceResponse = await _productService.DeleteProduct(id);
            if (serviceResponse.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(serviceResponse.Messages);
            }
            return NoContent();
        }

        //I will build this View in the future, unfortunetly I didn't have enough time to debug (Swagger is not working)
        /// <summary>
        /// Retrieves a list of products that belong to a specific category.
        /// </summary>
        /// <param name="category">The category name to filter products by.</param>
        /// <example>
        /// GET: /api/ProductAPI/ByCategory/Dairy
        /// </example>
        [HttpGet("ByCategory/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category cannot be empty."); 
            }

            var products = await _productService.GetProductsByCategory(category);
            if (products == null || !products.Any())
            {
                return NotFound("No products found for the specified category.");
            }
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var products = await _productService.SearchProducts(query);
            if (products == null || !products.Any())
            {
                return NotFound("No products found matching the search criteria.");
            }
            return Ok(products);
        }
    }
}

