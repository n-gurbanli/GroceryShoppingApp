using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryShoppingApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDesc { get; set; }

        public string Category { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        //Products can be in various(many) carts
        public ICollection<Cart> Carts { get; set; }

    }


    public class ProductDto
    {
        public string ProductName { get; set; }

        public string ProductDesc { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string Cart { get; set; }

    }
}
