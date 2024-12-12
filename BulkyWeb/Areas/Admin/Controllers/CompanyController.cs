using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles =StaticDetails.Admin_Role)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _IUOF;
		
		public CompanyController(IUnitOfWork IUOF)
		{
			_IUOF = IUOF;
			
		}
		public IActionResult Index()
		{
			List<Company> Companies = _IUOF.company.GetAll().ToList();
			return View(Companies);
		}
		 public IActionResult Upsert(int? id)
		{

			Company NewCompany = new();
			
			if(id==null || id == 0)
			{
				//Create
				return View(NewCompany);
			}
			else
			{
				//Update
				Company ExisitingCompany = _IUOF.company.Get(I => I.Id == id);
				return View(ExisitingCompany);
			}


		}
		[HttpPost]
		public IActionResult Upsert(Company obj )
		{
			if (obj.Name.ToLower() == "sisi")
			{
				ModelState.AddModelError("name", "Name can not be 'sisi' ");
			};
			
			
			if (ModelState.IsValid)
			{
				

				if (obj.Id == 0)
				{
					_IUOF.company.add(obj);
					TempData["success"] = "Company has been created succssefully";
				}
                else
                {
					_IUOF.company.Update(obj);
					TempData["success"] = "Company has been updated succssefully";
				}

                _IUOF.Save();
				
				return RedirectToAction("Index");
			}
			else
			{
				

				return View(obj);

			};

			}

		
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Company? ComapnyfromDb = _IUOF.company.Get(i => i.Id == id);
			if (ComapnyfromDb == null)
			{
				return NotFound();
			}
			return View(ComapnyfromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Company? DeletedCompany = _IUOF.company.Get(i => i.Id == id);
			if (DeletedCompany == null)
			{
				return NotFound();
			}
			_IUOF.company.Remove(DeletedCompany);
			_IUOF.Save();
			TempData["success"] = "Company has been deleted succssefully";
			return RedirectToAction("Index");


		}


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> CompaniesList = _IUOF.company.GetAll().ToList();

			return Json(new { Data = CompaniesList });
		}
		#endregion


	}

}
