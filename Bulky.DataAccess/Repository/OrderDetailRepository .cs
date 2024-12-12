using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Data;

namespace BulkyWeb.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail> , IOrderDetailRepository
    {
		private ApplicationDbContext _db;
		public OrderDetailRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		

		public void Update(OrderDetail obj)
		{
			_db.OrderDetails.Update(obj);
		}
	}
}
