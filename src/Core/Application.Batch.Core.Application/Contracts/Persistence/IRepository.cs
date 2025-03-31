using System.Linq.Expressions;

namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface IRepository<T> where T : class
{
	T? GetById(int id);
	Task<T?> GetByIdAsync(int id);
	List<T> GetAll();
	Task<List<T>> GetAllAsync();
	T Add(T entity);
	void AddRange(IEnumerable<T> entities);
	void Update(T entity);
	void Remove(T entity);
	void RemoveRange(IEnumerable<T> entities);
	void RemoveAll();
	List<T> Find(Expression<Func<T, bool>> predicate);
	Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
	T? Find(Expression<Func<T, bool>> predicate, List<string>? includes);
	Task<T?> FindAsync(Expression<Func<T, bool>> predicate, List<string>? includes);
	bool DoesExist(Expression<Func<T, bool>> predicate);
	Task<bool> DoesExistAsync(Expression<Func<T, bool>> predicate);
}