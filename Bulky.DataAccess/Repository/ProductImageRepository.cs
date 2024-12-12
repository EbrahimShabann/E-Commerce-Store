using BulkyWeb.Models;
using Bulky.DataAccess.Repository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.DataAccess.Data;

namespace BulkyWeb.Repository
{
    public class ProductImageRepository : Repository<ProductImage> ,IProductImageRepository
	{
		private ApplicationDbContext _db;
		public ProductImageRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		

		public void Update(ProductImage obj)
		{
			_db.ProductImages.Update(obj);
		}
	}
}
