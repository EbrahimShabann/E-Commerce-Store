using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using BulkyWeb.Models;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Admin_Role)]
	public class UserController : Controller
	{
       // Uri baseAddress = new Uri("http://localhost:5171/api");
        private readonly ApplicationDbContext _db;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly HttpClient _client;
		public UserController(ApplicationDbContext db , UserManager<IdentityUser> userManager , HttpClient client)
		{
			_db = db;
			_userManager = userManager;
			_client = client;
           // _client.BaseAddress = baseAddress;

        }
        public IActionResult Index()
		{
            List<ApplicationUser> UsersList =_db.ApplicationUsers.ToList();
           // HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/ApplicationUsers/GetApplicationUsers").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    string Data = response.Content.ReadAsStringAsync().Result;
            //    UsersList = JsonConvert.DeserializeObject<List<ApplicationUser>>(Data);
            //}
            return View(UsersList);
        }
		public IActionResult RoleManagment(string id)
		{
			string RoleId= _db.UserRoles.FirstOrDefault(u=>u.UserId==id).RoleId;

			UserVM userVM = new ()
			{
				ApplicationUser=_db.ApplicationUsers.Include(u=>u.Company).FirstOrDefault(u=>u.Id==id),
				RolesList= _db.Roles.Select(u=>new SelectListItem
				{
					Text=u.Name,
					Value=u.Id
				}),
				CompaniesList= _db.Companies.Select(u=>new SelectListItem
				{
					Text=u.Name,
					Value=u.Id.ToString()
				})

			};
			userVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
			return View(userVM);
	    }
		[HttpPost]
		public IActionResult RoleManagment(UserVM userVM)
		{
			string RoleId = _db.UserRoles.FirstOrDefault(u => u.UserId == userVM.ApplicationUser.Id).RoleId;
			string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleId).Name;
			string newRole = _db.Roles.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Role).Name;
			if (userVM.ApplicationUser.Role != oldRole)
			{
				//Role was updated
				ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.ApplicationUser.Id);
				if(newRole == StaticDetails.Company_Role)
				{
					applicationUser.CompanyId = userVM.ApplicationUser.CompanyId;
				}
				if(oldRole == StaticDetails.Company_Role)
				{
					applicationUser.CompanyId = null;
				}
				_db.SaveChanges();
				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				
				_userManager.AddToRoleAsync(applicationUser, newRole).GetAwaiter().GetResult();
			}
			return RedirectToAction("Index");

			
		}




		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> UsersList = _db.ApplicationUsers.Include(u=>u.Company).ToList();
			var userRoles = _db.UserRoles.ToList();
			var roles=_db.Roles.ToList();

            foreach (var user in UsersList)
            {
				var roleId=userRoles.FirstOrDefault(u=>u.UserId==user.Id).RoleId;
				user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                
            }

            return Json(new { Data = UsersList });
		}

		[HttpPost]
		public IActionResult LockUnLock([FromBody]string id) 
		{
			var ObjFromDb = _db.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
			if (ObjFromDb == null)
			{
				return Json(new {success = false ,message="Error while Locking/Unlocking"});
			}
			if( ObjFromDb.LockoutEnd!=null && ObjFromDb.LockoutEnd> DateTime.Now )
			{
				//user is currently locked and we need to unlock them
				ObjFromDb.LockoutEnd= DateTime.Now;
			}
			else
			{
				//user is unlocked and we need to lock them
				ObjFromDb.LockoutEnd=DateTime.Now.AddYears(100);
			}
			_db.SaveChanges();
			return Json(new { success = true, message = "Opeartion Succefully" });
		}
		#endregion


	}
}


