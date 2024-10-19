using GroceryShoppingApp.Models;
using GroceryShoppingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GroceryShoppingApp.Models.ViewModels;



namespace GroceryShoppingApp.Controllers
{
    public class ProductPageController : Controller
    {
        private readonly IProductService _productService;

        public ProductPageController(IProductService productService)
        {
            _productService = productService;
        }

        
        public async Task<IActionResult> List()
        {
            var products = await _productService.ListProducts();
            return View(products); 
        }

        // GET: ProductPage/New
        public IActionResult New()
        {
            return View();
        }

        // POST ProductPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(ProductDto ProductDto)
        {
            ServiceResponse response = await _productService.AddProduct(ProductDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", "ProductPage", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ProductPage/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.FindProduct(id);

            if (product == null)
            {
                return NotFound(); 
            }

            return View(product); 
        }

        // GET: ProductPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ProductDto? productDto = await _productService.FindProduct(id);
            if (productDto == null)
            {
                return View("Error"); 
            }
            return View(productDto); 
        }

        // POST: ProductPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductDto productDto)
        {


            ServiceResponse response = await _productService.UpdateProduct(productDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", "ProductPage", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages }); 
            }
        }


            // GET: ProductPage/ConfirmDelete/{id}
            [HttpGet]
            public async Task<IActionResult> ConfirmDelete(int id)
            {
                
                ProductDto? productDto = await _productService.FindProduct(id);

                if (productDto == null)
                {
                    
                    return View("Error", new ErrorViewModel() { Errors = new List<string> { "Product not found." } });
                }

                
                return View(productDto);
            }

            // POST: ProductPage/Delete/{id}
            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                
                ServiceResponse response = await _productService.DeleteProduct(id);

                if (response.Status == ServiceResponse.ServiceStatus.Deleted)
                {
                    
                    return RedirectToAction("List", "ProductPage");
                }
                else
                {
                    
                    return RedirectToAction("Error", new ErrorViewModel() { Errors = response.Messages });
                }
            }

       
    }
    }

