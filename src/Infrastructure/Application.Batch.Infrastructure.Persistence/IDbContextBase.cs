using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Batch.Infrastructure.Persistence;

public interface IDbContextBase : IDisposable
{
	IDatabaseFacadeWrapper Database { get; }
	public int SaveChanges();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
	EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
