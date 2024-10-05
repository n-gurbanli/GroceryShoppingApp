using GroceryShoppingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace GroceryShoppingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //customer.cs will map to Customers table
        public DbSet<Customer> Customers { get; set; }

        //prouct.cs will map to Products table
        public DbSet<Product> Products { get; set; }

        //cart.cs will map to Carts table
        public DbSet<Cart> Carts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
