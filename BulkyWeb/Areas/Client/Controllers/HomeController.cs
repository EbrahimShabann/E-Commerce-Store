using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Client.Controllers
{
    [Area("Client")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork=unitOfWork;
        }

        public IActionResult Index()
        {
            

            IEnumerable<Product> ProductList = _unitOfWork.product.GetAll(IncludedProperties: "Category,ProductImages");
            return View(ProductList);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart cart = new()
            {
                product = _unitOfWork.product.Get(p => p.Id == ProductId, IncludedProperties: "Category,ProductImages"),
                Count=1
            };
           
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity= (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.UserId=userId;
            
            ShoppingCart CartFromdb =_unitOfWork.shoppingcart.Get(c=>c.UserId==userId && c.ProductId==shoppingCart.ProductId);
            if (CartFromdb == null)
            {
                // add cart record
                _unitOfWork.shoppingcart.add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(StaticDetails.SessionCart,
                    _unitOfWork.shoppingcart.GetAll(c => c.UserId == userId).Count());

            }
            else
            {
                // cart exists
                CartFromdb.Count += shoppingCart.Count;
                _unitOfWork.shoppingcart.Update(CartFromdb);
                _unitOfWork.Save();
            }
            
           
            return RedirectToAction("Index");
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}




