using Bulky.Models.Models;
using BulkyWeb.Models;

namespace BulkyWeb.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader obj);
		void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null);
		void UpdateStripePaymentId(int id, string SessionId, string PaymentIntentId);
		
	}
}
