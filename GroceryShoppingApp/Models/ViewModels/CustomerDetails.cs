using System.Collections.Generic;
using GroceryShoppingApp.Models;

namespace GroceryShoppingApp.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public CustomerDto Customer { get; set; }
        public IEnumerable<CartDto> Carts { get; set; }
    }
}
