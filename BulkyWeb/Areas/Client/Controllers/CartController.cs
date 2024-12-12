using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyWeb.Repository;
using BulkyWeb.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ApplicationDbContext db;
        [BindProperty]
        public  ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork,IShoppingCartRepository shoppingCartRepository)
        {
            _unitOfWork=unitOfWork;
            _shoppingCartRepository = shoppingCartRepository;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var UserCart = _shoppingCartRepository.GetAll(s => s.UserId == userId);



            if (UserCart.IsNullOrEmpty() )
            {
                return RedirectToAction("EmptyCartList");
            }
            else
            {
                shoppingCartVM = new()
                {
                    ShoppingCartList =_shoppingCartRepository.GetAll(s => s.UserId == userId, IncludedProperties: "product"),
                    OrderHeader = new()
                };

                IEnumerable<ProductImage> productImages = _unitOfWork.productImage.GetAll();

                foreach (var cart in shoppingCartVM.ShoppingCartList)
                {
                    cart.product.ProductImages=productImages.Where(u=>u.ProductId==cart.ProductId).ToList();
                    cart.Price = PriceBasedOnCount(cart);
                    shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);
                }

                return View(shoppingCartVM);
            }

            
        }
        public IActionResult EmptyCartList()
        {
            return View();
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.shoppingcart.GetAll(s => s.UserId == userId, IncludedProperties: "product").ToList(),
                OrderHeader = new()
            };
            shoppingCartVM.OrderHeader.ApplicationUser=_unitOfWork.applicationUser.Get(u=>u.Id == userId);
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.Street = shoppingCartVM.OrderHeader.ApplicationUser.Street;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.Country = shoppingCartVM.OrderHeader.ApplicationUser.Country;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = PriceBasedOnCount(cart);
                shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);
            }
            return View(shoppingCartVM);
        }


        [HttpPost]
        [ActionName("Summary")]
		public IActionResult SummaryPost()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList=_unitOfWork.shoppingcart.GetAll(s => s.UserId == userId, IncludedProperties: "product").ToList();
            shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId=userId;
			ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(u => u.Id == userId);
			

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = PriceBasedOnCount(cart);
				shoppingCartVM.OrderHeader.TotalOrder += (cart.Price * cart.Count);
			}

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
				// Customer user  
				shoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
                shoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            }
            else
            {
                // Company user
                shoppingCartVM.OrderHeader.OrderStatus= StaticDetails.StatusApproved;
                shoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
            }

            _unitOfWork.orderHeader.add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    ProductId = cart.ProductId,
                    Count = cart.Count,
                    Price = cart.Price

                };
                _unitOfWork.orderDetail.add(orderDetail);
                _unitOfWork.Save();
            }
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                // Customer user and i have to capture payment 
                //Stripe Logic
                var domain = "https://localhost:7054/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain+ $"Client/Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl= domain+"Client/Cart/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //$20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);

				}
                var service = new SessionService();
				Session session= service.Create(options);
                _unitOfWork.orderHeader.UpdateStripePaymentId(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}
			return RedirectToAction("OrderConfirmation", new {id=shoppingCartVM.OrderHeader.Id});
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader=_unitOfWork.orderHeader.Get(o=>o.Id==id,IncludedProperties: "ApplicationUser");
            if(orderHeader.PaymentStatus!= StaticDetails.PaymentStatusDelayedPayment)
            {
                // This order is by a customer user
                var service= new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid") {
					_unitOfWork.orderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					_unitOfWork.orderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
					_unitOfWork.Save();
				}
                HttpContext.Session.Clear();
			}
            List<ShoppingCart> ShoppingCartList = _unitOfWork.shoppingcart
                .GetAll(s => s.UserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.shoppingcart.RemoveRange(ShoppingCartList);
            _unitOfWork.Save();
			return View(id);
		}




           
        
		public IActionResult plus(int cartId)
        {
           var cartfromDb = _unitOfWork.shoppingcart.Get(s=>s.Id== cartId);
            cartfromDb.Count += 1;
            _unitOfWork.shoppingcart.Update(cartfromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        public IActionResult minus(int cartId)
        {
           var cartfromDb = _unitOfWork.shoppingcart.Get(s=>s.Id== cartId);
            if(cartfromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.shoppingcart.GetAll(c => c.UserId == cartfromDb.UserId).Count()-1);

                _unitOfWork.shoppingcart.Remove(cartfromDb);
            }
            else
            {
                cartfromDb.Count -= 1;
                
                _unitOfWork.shoppingcart.Update(cartfromDb);
            }
           
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        public IActionResult remove(int cartId)
        {
           var cartfromDb = _unitOfWork.shoppingcart.Get(s=>s.Id== cartId);
            HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.shoppingcart.GetAll(c => c.UserId == cartfromDb.UserId).Count()-1);

            _unitOfWork.shoppingcart.Remove(cartfromDb);
            _unitOfWork.Save();
            return RedirectToAction("Index");

        }
        
        private double PriceBasedOnCount(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.product.Price1;
            }
            else if(shoppingCart.Count <= 100)
            {
                return shoppingCart.product.Price50;
            }
            else
            {
                return shoppingCart.product.Price100;
            }
        }
    }

    
}
