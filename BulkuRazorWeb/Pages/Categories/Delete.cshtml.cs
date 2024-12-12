using BulkuRazorWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkuRazorWeb.Pages.Categories
{
	[BindProperties]

	public class DeleteModel : PageModel
	{
		private readonly ApplicationDbContext _db;
		public Category Category { get; set; }
		public DeleteModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			if (id != 0 && id != null)
			{
				Category = _db.Categories.Find(id);

			}

		}
		public IActionResult OnPost()
		{
			Category? DeletedCategory = _db.Categories.Find(Category.Id);
			if (DeletedCategory == null)
			{
				return NotFound();
			}
			_db.Categories.Remove(DeletedCategory);
			_db.SaveChanges();
			return RedirectToPage("Index");
		
			return Page();

		}
	}
}
