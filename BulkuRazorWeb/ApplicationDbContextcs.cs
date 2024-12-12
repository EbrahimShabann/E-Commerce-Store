using Microsoft.EntityFrameworkCore;
using BulkuRazorWeb.Models;

namespace BulkuRazorWeb
{
	public class ApplicationDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BulkyRazor;Integrated Security=True");
		}
		public DbSet<Category> Categories { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Men", DisplayOrder = 1 },
				new Category { Id = 2, Name = "Women", DisplayOrder = 2 },
				new Category { Id = 3, Name = "Children", DisplayOrder = 3 }
				);
		}

	}
}
