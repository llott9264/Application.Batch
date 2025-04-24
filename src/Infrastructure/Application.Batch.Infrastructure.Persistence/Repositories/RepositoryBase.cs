using System.Linq.Expressions;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T>(IDbContextBase context) : IRepositoryBase<T> where T : Entity
{
	public T? GetById(int id)
	{
		return context.Set<T>().Find(id);
	}

	public async Task<T?> GetByIdAsync(int id)
	{
		return await context.Set<T>().FindAsync(id);
	}

	public List<T> GetAll()
	{
		return context.Set<T>().ToList();
	}

	public async Task<List<T>> GetAllAsync()
	{
		return await context.Set<T>().ToListAsync();
	}

	public T Add(T entity)
	{
		context.Set<T>().Add(entity);
		return entity;
	}

	public void AddRange(IEnumerable<T> entities)
	{
		context.Set<T>().AddRange(entities);
	}

	public void Update(T entity)
	{
		context.Set<T>().Update(entity);
	}

	public void Remove(T entity)
	{
		context.Set<T>().Remove(entity);
	}

	public void RemoveRange(IEnumerable<T> entities)
	{
		context.Set<T>().RemoveRange(entities);
	}

	public void RemoveAll()
	{
		List<T> entities = GetAll();
		RemoveRange(entities);
	}

	public List<T> Find(Expression<Func<T, bool>> predicate)
	{
		return context.Set<T>().Where(predicate).ToList();
	}

	public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
	{
		return await context.Set<T>().Where(predicate).ToListAsync();
	}

	public T? Find(Expression<Func<T, bool>> predicate, List<string>? includes)
	{
		IQueryable<T> query = context.Set<T>();
		if (includes != null)
		{
			query = includes.Aggregate(query, (current, include) => current.Include(include));
		}

		return query.FirstOrDefault(predicate);
	}

	public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, List<string>? includes)
	{
		IQueryable<T> query = context.Set<T>();
		if (includes != null)
		{
			query = includes.Aggregate(query, (current, include) => current.Include(include));
		}

		return await query.FirstOrDefaultAsync(predicate);
	}

	public bool DoesExist(Expression<Func<T, bool>> predicate)
	{
		return context.Set<T>().Any(predicate);
	}

	public async Task<bool> DoesExistAsync(Expression<Func<T, bool>> predicate)
	{
		return await context.Set<T>().AnyAsync(predicate);
	}
}
