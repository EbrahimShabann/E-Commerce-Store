using Bulky.Models.Models;
using BulkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IProductRepository :IRepository<Product>
	{
		void Update(Product product);
	}
}
