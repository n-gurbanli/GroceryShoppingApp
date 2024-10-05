using System.ComponentModel.DataAnnotations;

namespace GroceryShoppingApp.Models
{
    public class Customer
    {
        //making sure Customer is registered with primary key
        [Key]
        public int CustomerId { get; set; }

        public string CustomerFname { get; set; }

        public string CustomerLname { get; set; }

        //this is sensitive info, do not output
        //I wanted to keep addresses out of Dto bt when i create or update customer I dont see the address,so i cant update it
        //thats why i decided to include it to Dto, I dont know if it would have been correct not to update it
        public string CustomerAddress { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        //Customer(user) can have many carts

        public ICollection<Cart>? Carts { get; set; }

    }

    public class CustomerDto
    {
        public string CustomerLname { get; set; }

        public string CustomerFname { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerNum { get; set; }


    }
}
