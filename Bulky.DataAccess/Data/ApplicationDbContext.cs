using Bulky.Models.Models;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Bulky;Integrated Security=True");
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Men", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Women", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Children", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Pants", Brand = "Gucci", ListPrice = 50, Price1 = 85, Price50 = 750, Price100 = 2355, CategoryId = 1, Description = "men Clothes" },
                new Product { Id = 2, Name = "Shirts", Brand = "Gucci", ListPrice = 55, Price1 = 90, Price50 = 800, Price100 = 2500, CategoryId = 2, Description = "Women Clothes" },
                new Product { Id = 3, Name = "T-Shirts", Brand = "Gucci", ListPrice = 45, Price1 = 75, Price50 = 750, Price100 = 2300, CategoryId = 1, Description = "men Clothes" },
                new Product { Id = 4, Name = "Shorts", Brand = "Gucci", ListPrice = 30, Price1 = 65, Price50 = 600, Price100 = 2245, CategoryId = 2, Description = "Women Clothes" }
                );

            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Rexona", StreetAddress = "GehanSt", City = "Mansoura", Country = "Egypt", PhoneNumber = "0502026521" },
                new Company { Id = 2, Name = "Sack", StreetAddress = "El-MargSt", City = "Cairo", Country = "Egypt", PhoneNumber = "0502298771" },
                new Company { Id = 3, Name = "Kemoo", StreetAddress = "El-Ma3radSt", City = "Tanta", Country = "Egypt", PhoneNumber = "0503658013" }
                );


        }

    }
}
