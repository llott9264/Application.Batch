using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Batch.Infrastructure.Persistence;

public interface IDbContextBase : IDisposable
{
	DatabaseFacade Database { get; }
	public int SaveChanges();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
	EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
