using System.ComponentModel.DataAnnotations;
namespace GroceryShoppingApp.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public string CartName { get; set; }

        public DateTime DateCreated { get; set; }

        //One cart can have only one Customer(user)

        public int CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

        //Carts can contain many products

        public ICollection<Product> Products { get; set; }


    }


    public class CartDto
    {
        public int CartId { get; set; }

        public string CartName { get; set; }

        public DateTime DateCreated { get; set; }

        public List<ProductDto> Products { get; set; }

        public string CartCustomer { get; set; }

        public int CustomerId { get; set; }


    }
}
