using BulkuRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkuRazorWeb.Pages.Categories
{
	[BindProperties]

	public class editModel : PageModel
	{
		private readonly ApplicationDbContext _db;
		public Category Category { get; set; }
		public editModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			if(id != 0 && id != null)
			{
				Category = _db.Categories.Find(id);

			}

		}
		public IActionResult OnPost()
		{



			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category);
				_db.SaveChanges();
				//TempData["success"] = "Category has been updated succssefully";
				return RedirectToPage("Index");
			}
			return Page();

		}
	}
}
