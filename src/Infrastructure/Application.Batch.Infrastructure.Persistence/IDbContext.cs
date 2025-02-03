using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Batch.Infrastructure.Persistence;

public interface IDbContext : IDisposable
{
	public DbSet<Address> Addresses { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public int SaveChanges();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
	EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
	DatabaseFacade Database { get; }
}