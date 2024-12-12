using BulkyWeb.Models;

namespace BulkyWeb.Repository.IRepository
{
	public interface ICategoryRepository : IRepository<Category>
	{
		void Update(Category obj);
		
	}
}
