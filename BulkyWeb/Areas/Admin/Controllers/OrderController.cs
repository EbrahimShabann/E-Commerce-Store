using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitofwork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitofwork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult RemoveCancelledOrders()
		{
			IEnumerable<OrderHeader> orderHeaders;
			orderHeaders = _unitofwork.orderHeader.GetAll(u => u.OrderStatus == StaticDetails.StatusCancelled);
			_unitofwork.orderHeader.RemoveRange(orderHeaders);
			_unitofwork.Save();
			return RedirectToAction("Index");
        }
		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitofwork.orderHeader.Get(u => u.Id == orderId, IncludedProperties: "ApplicationUser"),
				OrderDetail = _unitofwork.orderDetail.GetAll(u => u.OrderHeaderId == orderId, IncludedProperties: "Product")
			};
			return View(OrderVM);
		}
		[HttpPost]
		[Authorize(Roles = StaticDetails.Admin_Role + "," + StaticDetails.Employee_Role)]
		public IActionResult UpdateOrderDetails()
		{
			var OrderHeaderFromDb = _unitofwork.orderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			OrderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			OrderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			OrderHeaderFromDb.Street = OrderVM.OrderHeader.Street;
			OrderHeaderFromDb.City = OrderVM.OrderHeader.City;
			OrderHeaderFromDb.Country = OrderVM.OrderHeader.Country;

			if (!String.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{
				OrderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (!String.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
			{
				OrderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitofwork.orderHeader.Update(OrderHeaderFromDb);
			_unitofwork.Save();
			TempData["Success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), new { orderId = OrderHeaderFromDb.Id });
		}


		[HttpPost]
		[Authorize(Roles = StaticDetails.Admin_Role + "," + StaticDetails.Employee_Role)]

		public IActionResult StartProcessing()
		{
			_unitofwork.orderHeader.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
			_unitofwork.Save();
			TempData["Success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}
		[HttpPost]
		[Authorize(Roles = StaticDetails.Admin_Role + "," + StaticDetails.Employee_Role)]

		public IActionResult ShippingOrder()
		{
			var orderHeader = _unitofwork.orderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = StaticDetails.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;

			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
			}
			_unitofwork.orderHeader.Update(orderHeader);
			_unitofwork.Save();
			TempData["Success"] = "Order Shipped Successfully";

			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}
		[HttpPost]
		[Authorize(Roles = StaticDetails.Admin_Role + "," + StaticDetails.Employee_Role)]

		public IActionResult CancelOrder()
		{
			var orderHeader = _unitofwork.orderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntedId

				};
				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitofwork.orderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);

			}
			else
			{
				_unitofwork.orderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
			}
			_unitofwork.Save();
			TempData["Success"] = "Order Cancelled Successfully";

			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });

		}

		[HttpPost]
		[ActionName("Details")]
		public IActionResult Details_Pay_Now()
		{
			OrderVM.OrderHeader = _unitofwork.orderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, IncludedProperties: "ApplicationUser");

			OrderVM.OrderDetail = _unitofwork.orderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, IncludedProperties: "Product");
				 // Company user and i have to capture payment 
				 //Stripe Logic
				var domain = "https://localhost:44311/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?OrderHeaderId={OrderVM.OrderHeader.Id}",
				CancelUrl = domain + $"Admin/Order/Details?orderId={OrderVM.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in OrderVM.OrderDetail)
			{
				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100), //$20.50 => 2050
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);

			}
			var service = new SessionService();
			Session session = service.Create(options);
			_unitofwork.orderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitofwork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
	
        }
        public IActionResult PaymentConfirmation(int OrderHeaderId)
        {
            OrderHeader orderHeader = _unitofwork.orderHeader.Get(o => o.Id == OrderHeaderId);
            if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
            {
                // This order is by a company user
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitofwork.orderHeader.UpdateStripePaymentId(OrderHeaderId, session.Id, session.PaymentIntentId);
                    _unitofwork.orderHeader.UpdateStatus(OrderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
                    _unitofwork.Save();
                }
            }
           

            
            return View(OrderHeaderId);
        }





        #region API CALLS
        [HttpGet]
		public IActionResult GetAll(string status)
		{
	
			IEnumerable<OrderHeader> objOrderHeaders ;

			if(User.IsInRole(StaticDetails.Admin_Role)||User.IsInRole(StaticDetails.Employee_Role))
			{
				objOrderHeaders = _unitofwork.orderHeader.GetAll(IncludedProperties: "ApplicationUser").ToList();

            }
			else
			{
				var claimsIdentity= (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				objOrderHeaders = _unitofwork.orderHeader.GetAll(u => u.ApplicationUserId == userId, IncludedProperties: "ApplicationUser");
			}
			switch (status)
			{
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
					break;
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusShipped); 
					break;
				case "cancelled":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == StaticDetails.StatusCancelled); 
					break;
				default:
					break;

			}

			return Json(new { Data = objOrderHeaders });
		}
		#endregion
	}
}
