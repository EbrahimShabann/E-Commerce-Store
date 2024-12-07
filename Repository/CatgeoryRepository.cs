using BulkyWeb.Data;
using BulkyWeb.Models;
using Bulky.DataAccess.Repository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
	public class CatgeoryRepository : Repository<Category> ,ICategoryRepository
	{
		private ApplicationDbContext _db;
		public CatgeoryRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		

		public void Update(Category obj)
		{
			_db.Categories.Update(obj);
		}
	}
}
