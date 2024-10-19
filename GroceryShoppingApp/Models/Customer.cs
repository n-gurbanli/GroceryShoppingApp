using System.ComponentModel.DataAnnotations;

namespace GroceryShoppingApp.Models
{
    public class Customer
    {
        
        [Key]
        public int CustomerId { get; set; }

        public string CustomerFname { get; set; }

        public string CustomerLname { get; set; }

        
        public string CustomerAddress { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        //Customer(user) can have many carts

        public ICollection<Cart>? Carts { get; set; }

    }

    public class CustomerDto
    {
        public int CustomerId { get; set; }
        
        public string CustomerLname { get; set; }

        public string CustomerFname { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerNum { get; set; }


    }
}
