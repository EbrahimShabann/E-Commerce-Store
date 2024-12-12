using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Admin_Role)] 
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5171/api");  
        private readonly IUnitOfWork _IUOF;
        private readonly HttpClient _client;
        public CategoryController(IUnitOfWork IUOF , HttpClient client)
        {
            _client = client;
            _client.BaseAddress = baseAddress;
            _IUOF = IUOF;
           
            
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Category> CategoriesList = new List<Category>();
            HttpResponseMessage response=_client.GetAsync(_client.BaseAddress + "/Categories/GetCategories").Result;
            if (response.IsSuccessStatusCode)
            {
                string Data = response.Content.ReadAsStringAsync().Result;
                CategoriesList = JsonConvert.DeserializeObject<List<Category>>(Data);
            }
            return View(CategoriesList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name.ToLower() == "sisi")
            {
                ModelState.AddModelError("name", "Name can not be 'sisi' ");
            };
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display Order can not be the same as Name");
            }
            if (ModelState.IsValid)
            {
                _IUOF.category.add(obj);
               // _db.Categories.OrderBy(c => c.DisplayOrder);
                _IUOF.Save();
                TempData["success"] = "Category has been created succssefully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? CategoryfromDb = _IUOF.category.Get(i=>i.Id==id);
            if (CategoryfromDb == null)
            {
                return NotFound();
            }
            return View(CategoryfromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name.ToLower() == "sisi")
            {
                ModelState.AddModelError("name", "Name can not be 'sisi' ");
            };
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Display Order can not be the same as Name");
            }
            if (ModelState.IsValid)
            {
                _IUOF.category.Update(obj);
                _IUOF.Save();
                TempData["success"] = "Category has been updated succssefully";
                return RedirectToAction("Index");
            }
            return View();

        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? CategoryfromDb = _IUOF.category.Get(i => i.Id == id);
			if (CategoryfromDb == null)
            {
                return NotFound();
            }
            return View(CategoryfromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? DeletedCategory = _IUOF.category.Get(i => i.Id == id);
			if (DeletedCategory == null)
            {
                return NotFound();
            }
            _IUOF.category.Remove(DeletedCategory);
            _IUOF.Save();
            TempData["success"] = "Category has been deleted succssefully";
            return RedirectToAction("Index");


        }




    }
}
