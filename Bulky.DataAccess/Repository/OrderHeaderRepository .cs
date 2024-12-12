using BulkyWeb.Models;
using BulkyWeb.Repository.IRepository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Data;

namespace BulkyWeb.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader> , IOrderHeaderRepository
    {
		private ApplicationDbContext _db;
		public OrderHeaderRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		

		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}

		public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null)
		{
			var OrderFromDb=_db.OrderHeaders.FirstOrDefault(o=>o.Id== id);
			if (OrderFromDb!=null)
			{
				OrderFromDb.OrderStatus = OrderStatus;
				if(!string.IsNullOrEmpty(PaymentStatus)) 
				{ 
					OrderFromDb.PaymentStatus = PaymentStatus;
				}
			}
		}

		public void UpdateStripePaymentId(int id, string SessionId, string PaymentIntentId)
		{
			var OrderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (!string.IsNullOrEmpty(SessionId))
            {
				OrderFromDb.SessionId = SessionId;
            }
			if (!string.IsNullOrEmpty(PaymentIntentId))
			{
				OrderFromDb.PaymentIntedId = PaymentIntentId;
				OrderFromDb.PaymentDate=DateTime.Now;
			}
        }
	}
}
