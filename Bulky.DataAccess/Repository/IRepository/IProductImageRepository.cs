using Bulky.Models.Models;
using BulkyWeb.Models;

namespace BulkyWeb.Repository.IRepository
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{
		void Update(ProductImage obj);
		
	}
}
