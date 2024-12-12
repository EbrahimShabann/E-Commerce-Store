using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles =StaticDetails.Admin_Role)]
	public class ProductController : Controller
	{
		Uri baseAddress = new Uri("http://localhost:5171/api");
		private readonly HttpClient _client;
		private readonly IUnitOfWork _IUOF;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork IUOF, IWebHostEnvironment webHostEnvironment)
		{
			_client= new HttpClient();
			_client.BaseAddress = baseAddress;
			_IUOF = IUOF;
			_webHostEnvironment = webHostEnvironment;
		}
		[HttpGet]
		public IActionResult Index()
		{
			List<Product> ProductsList = new List<Product>();
			HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Products/GetProducts").Result;
			if (response.IsSuccessStatusCode)
			{
				string Data = response.Content.ReadAsStringAsync().Result;
				ProductsList = JsonConvert.DeserializeObject<List<Product>>(Data);
			}
			return View(ProductsList);
		}
		public IActionResult Upsert(int? id)
		{

			//ViewBag.CategoryList = CategoryList;
			ProductVM ProdctVm = new()
			{
				CategoryList = _IUOF.category.GetAll()
				.Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()

			};
			if(id==null || id == 0)
			{
				//Create
				return View(ProdctVm);
			}
			else
			{
				//Update
				ProdctVm.Product = _IUOF.product.Get(u => u.Id == id, IncludedProperties: "ProductImages");
				return View(ProdctVm);
			}

			
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM obj ,List<IFormFile>? files)
		{
			if (obj.Product.Name.ToLower() == "sisi")
			{
				ModelState.AddModelError("name", "Name can not be 'sisi' ");
			};
			
			
			if (ModelState.IsValid)
			{
				if (obj.Product.Id == 0)
				{
					_IUOF.product.add(obj.Product);
					TempData["success"] = "Product has been created succssefully";
				}
				else
				{
					_IUOF.product.Update(obj.Product);
					TempData["success"] = "Product has been updated succssefully";
				}

				_IUOF.Save();


				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null)
				{
                    foreach (IFormFile file in files)
                    {
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\products\product-" + obj.Product.Id;
						string FinalPath = Path.Combine(wwwRootPath, productPath);

						if (!Directory.Exists(FinalPath))
						  Directory.CreateDirectory(FinalPath); 

						using (var fileStream = new FileStream(Path.Combine(FinalPath, fileName), FileMode.Create))
						{
							file.CopyTo(fileStream);
						}

						ProductImage productImage = new()
						{
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = obj.Product.Id,

						};

						if (obj.Product.ProductImages == null)
							obj.Product.ProductImages = new List<ProductImage>();

						obj.Product.ProductImages.Add(productImage);
					}
					_IUOF.product.Update(obj.Product);
					_IUOF.Save();



					
				}

				
				
				return RedirectToAction("Index");
			}
			else
			{
				obj.CategoryList = _IUOF.category.GetAll()
				.Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				});

				return View(obj);

			};

			}

		public IActionResult DeleteImage(int imageId)
		{
			var ImageToBeDeleted = _IUOF.productImage.Get(u=>u.Id==imageId);
			var productID = ImageToBeDeleted.ProductId;
			if (ImageToBeDeleted != null)
			{
				if (!string.IsNullOrEmpty(ImageToBeDeleted.ImageUrl))
				{
					var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, ImageToBeDeleted.ImageUrl.TrimStart('\\'));
					if(System.IO.File.Exists(oldPath))
						System.IO.File.Delete(oldPath);
				}
				_IUOF.productImage.Remove(ImageToBeDeleted);
				_IUOF.Save();
				TempData["success"] = "Image has been deleted succssefully";

			}
			return RedirectToAction("Upsert", new { id = productID });

		}

		

		

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? ProductfromDb = _IUOF.product.Get(i => i.Id == id);
			if (ProductfromDb == null)
			{
				return NotFound();
			}
			return View(ProductfromDb);
		}


		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Product? DeletedProduct = _IUOF.product.Get(i => i.Id == id);
			if (DeletedProduct == null)
			{
				return NotFound();
			}

			string productPath = @"images\products\product-" + id;
			string FinalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

			if (Directory.Exists(FinalPath))
			{
				//string[] filePaths = Directory.GetFiles(FinalPath);                          No need because when we delete the directory it also deletes all files in it 
    //            foreach (string filePath in filePaths)
    //            {
				//	System.IO.File.Delete(filePath);
    //            }


                Directory.Delete(FinalPath);
			}
				


			_IUOF.product.Remove(DeletedProduct);
			_IUOF.Save();
			TempData["success"] = "Product has been deleted succssefully";
			return RedirectToAction("Index");


		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll() 
		{
			List<Product> ProductsList = _IUOF.product.GetAll(IncludedProperties: "Category").ToList();

			return Json(new { Data = ProductsList });
		}
		#endregion


	}

}
//public IActionResult Edit(int? id)
//{
//	if (id == null || id == 0)
//	{
//		return NotFound();
//	}

//	Product? ProductfromDb = _IUOF.product.Get(i => i.Id == id);
//	if (ProductfromDb == null)
//	{
//		return NotFound();
//	}
//	ProductVM ProdctVm = new()
//	{
//		CategoryList = _IUOF.category.GetAll()
//		.Select(u => new SelectListItem
//		{
//			Text = u.Name,
//			Value = u.Id.ToString()
//		}),
//		Product = ProductfromDb

//	};
//	return View(ProdctVm);
//}
//[HttpPost]
//public IActionResult Edit(ProductVM obj)
//{
//	if (obj.Product.Name.ToLower() == "sisi")
//	{
//		ModelState.AddModelError("name", "Name can not be 'sisi' ");
//	};

//	if (ModelState.IsValid)
//	{
//		_IUOF.product.Update(obj.Product);
//		_IUOF.Save();
//		TempData["success"] = "Product has been updated succssefully";
//		return RedirectToAction("Index");
//	}
//	return View();

//}

