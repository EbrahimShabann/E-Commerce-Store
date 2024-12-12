using BulkyWeb.Models;
using Bulky.DataAccess.Repository;
using System.Linq.Expressions;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.DataAccess.Data;

namespace BulkyWeb.Repository
{
    public class CompanyRepository : Repository<Company> , ICompanyRepository
    {
		private ApplicationDbContext _db;
		public CompanyRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		

		public void Update(Company obj)
		{
			_db.Companies.Update(obj);
		}
	}
}
