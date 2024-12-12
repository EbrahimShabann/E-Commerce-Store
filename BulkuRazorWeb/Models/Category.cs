using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkuRazorWeb.Models
{
	public class Category
	{
		public int Id { get; init; }

		[Required]
		[MaxLength(30)]
		[DisplayName("Category Name")]
		public string? Name { get; init; }
		[Range(1, 100)]
		[DisplayName("Display Order")]
		public int DisplayOrder { get; init; }
	}
}
