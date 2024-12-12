using System.Linq.Expressions;

namespace BulkyWeb.Repository.IRepository
{
	public interface IRepository<T> where T: class
	{
		// T: Category or product or company etc
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? IncludedProperties=null);
		T Get(Expression<Func<T, bool>> filter, string? IncludedProperties = null, bool tracked = false);
		void add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
	}
}
