using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Common;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(IDbContext context) : IRepository<T> where T : Entity
{
	protected readonly IDbContext Context = context;

	public T? GetById(int id)
	{
		return Context.Set<T>().Find(id);
	}

	public async Task<T?> GetByIdAsync(int id)
	{
		return await Context.Set<T>().FindAsync(id);
	}

	public List<T> GetAll()
	{
		return Context.Set<T>().ToList();
	}

	public async Task<List<T>> GetAllAsync()
	{
		return await Context.Set<T>().ToListAsync();
	}

	public T Add(T entity)
	{
		Context.Set<T>().Add(entity);
		return entity;
	}

	public void AddRange(IEnumerable<T> entities)
	{
		Context.Set<T>().AddRange(entities);
	}

	public void Update(T entity)
	{
		Context.Set<T>().Update(entity);
	}

	public void Remove(T entity)
	{
		Context.Set<T>().Remove(entity);
	}

	public void RemoveRange(IEnumerable<T> entities)
	{
		Context.Set<T>().RemoveRange(entities);
	}

	public void RemoveAll()
	{
		List<T> entities = GetAll();
		RemoveRange(entities);
	}

	public List<T> Find(Expression<Func<T, bool>> predicate)
	{
		return Context.Set<T>().Where(predicate).ToList();
	}

	public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
	{
		return await Context.Set<T>().Where(predicate).ToListAsync();
	}

	public T? Find(Expression<Func<T, bool>> predicate, List<string>? includes)
	{
		IQueryable<T> query = Context.Set<T>();
		if (includes != null)
		{
			query = includes.Aggregate(query, (current, include) => current.Include(include));
		}

		return query.FirstOrDefault(predicate);
	}

	public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, List<string>? includes)
	{
		IQueryable<T> query = Context.Set<T>();
		if (includes != null)
		{
			query = includes.Aggregate(query, (current, include) => current.Include(include));
		}

		return await query.FirstOrDefaultAsync(predicate);
	}

	public bool DoesExist(Expression<Func<T, bool>> predicate)
	{
		return Context.Set<T>().Any(predicate);
	}

	public async Task<bool> DoesExistAsync(Expression<Func<T, bool>> predicate)
	{
		return await Context.Set<T>().AnyAsync(predicate);
	}
}