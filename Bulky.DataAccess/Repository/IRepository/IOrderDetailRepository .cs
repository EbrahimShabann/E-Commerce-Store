using Bulky.Models.Models;
using BulkyWeb.Models;

namespace BulkyWeb.Repository.IRepository
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{
		void Update(OrderDetail obj);
		
	}
}
