using Bulky.Models.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace BulkyWeb.Repository.IRepository
{
	public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
        IEnumerable<ShoppingCart> GetAll(Expression<Func<ShoppingCart, bool>> filter , string? IncludedProperties = null, bool tracked = false);

        void Update(ShoppingCart obj);
		
	}
}
