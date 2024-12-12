using BulkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		public ICategoryRepository category { get; }
		public IProductRepository product { get; }
		public ICompanyRepository company { get; }
		public IShoppingCartRepository shoppingcart { get; }
		public IApplicationUserRepository applicationUser { get; }
		public IOrderHeaderRepository orderHeader { get; }
		public IOrderDetailRepository orderDetail { get; }
		public IProductImageRepository productImage { get; }
		void Save();
	}
}
