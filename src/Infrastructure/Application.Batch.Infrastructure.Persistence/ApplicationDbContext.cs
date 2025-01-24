using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Common;
using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Batch.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
	public DbSet<Address> Addresses { get; set; }
	public DbSet<Customer> Customers { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
	{
		foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedDate = DateTime.Now;
					break;
				case EntityState.Modified:
					entry.Entity.LastModifiedDate = DateTime.Now;
					break;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}

	public override int SaveChanges()
	{
		foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedDate = DateTime.Now;
					break;
				case EntityState.Modified:
					entry.Entity.LastModifiedDate = DateTime.Now;
					break;
			}
		}

		return base.SaveChanges();
	}
}