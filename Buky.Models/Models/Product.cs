using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.Models
{
	public class Product
	{
		public int Id { get; set; }

		[MaxLength(30)]
		public string Name { get; set; }
        public string? Brand { get; set; }

        public string Description { get; set; }

        [Display(Name = "List Price")]
		[Range(1,1000)]
        public double ListPrice { get; set; }

		[Display(Name = "Price of 1:50 piece")]
		[Range(1,1000)]
        public double Price1 { get; set; }
		
		[Display(Name = "Price of +50 piece")]
		[Range(1,1000)]
        public double Price50 { get; set; }
		
		[Display(Name = "Price of +100 piece")]
		[Range(1,1000)]
        public double Price100 { get; set; }
		
		
		public int CategoryId { get; set; }
		[ForeignKey(nameof(CategoryId))]
		[ValidateNever]
        public Category Category { get; set; }
		[ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
