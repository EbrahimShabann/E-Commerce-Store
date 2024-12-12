using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Bulky.DataAccess.Data;

namespace BulkyWeb.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart> , IShoppingCartRepository
    {
		private ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}


        

            public void Update(ShoppingCart obj)
		{
			_db.ShoppingCarts.Update(obj);
		}

        IEnumerable<ShoppingCart> IShoppingCartRepository.GetAll(Expression<Func<ShoppingCart, bool>> filter, string? IncludedProperties, bool tracked)
        {
            IQueryable<ShoppingCart> query;

            if (tracked)
            {

                query = dbset;
            }
            else
            {
                query = dbset.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(IncludedProperties))
            {
                foreach (var IncludedPropi in IncludedProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(IncludedPropi);
                }
            }
            return query.ToList();
        }
    }
}
