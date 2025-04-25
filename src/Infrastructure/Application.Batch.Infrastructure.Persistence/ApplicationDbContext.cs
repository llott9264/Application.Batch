using System.Security.Principal;
using Application.Batch.Core.Domain.Common;
using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Batch.Infrastructure.Persistence;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: DbContext(options), IDbContext
{
	public new IDatabaseFacadeWrapper Database => new DatabaseFacadeWrapper(base.Database);
	public virtual DbSet<Address> Addresses { get; set; }
	public virtual DbSet<Customer> Customers { get; set; }

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
	{
		foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedDate = DateTime.Now;
					entry.Entity.CreatedBy = WindowsIdentity.GetCurrent().Name;
					break;
				case EntityState.Modified:
					entry.Entity.LastModifiedDate = DateTime.Now;
					entry.Entity.LastModifiedBy = WindowsIdentity.GetCurrent().Name;
					break;
			}
		}

		return await base.SaveChangesAsync(cancellationToken);
	}

	public override int SaveChanges()
	{
		foreach (EntityEntry<Entity> entry in ChangeTracker.Entries<Entity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedDate = DateTime.Now;
					entry.Entity.CreatedBy = WindowsIdentity.GetCurrent().Name;
					break;
				case EntityState.Modified:
					entry.Entity.LastModifiedDate = DateTime.Now;
					entry.Entity.LastModifiedBy = WindowsIdentity.GetCurrent().Name;
					break;
			}
		}

		return base.SaveChanges();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Address>(entity =>
		{
			entity.Property(e => e.City)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.CreatedBy)
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.LastModifiedBy)
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");
			entity.Property(e => e.State)
				.IsRequired()
				.HasMaxLength(2)
				.IsUnicode(false)
				.IsFixedLength();
			entity.Property(e => e.Street)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.ZipCode)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false)
				.IsFixedLength();

			entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
				.HasForeignKey(d => d.CustomerId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_Address_Customer");
		});

		modelBuilder.Entity<Customer>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_Customer");

			entity.Property(e => e.CreatedBy)
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.CreatedDate).HasColumnType("datetime");
			entity.Property(e => e.FirstName)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.LastModifiedBy)
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.LastModifiedDate).HasColumnType("datetime");
			entity.Property(e => e.LastName)
				.IsRequired()
				.HasMaxLength(100)
				.IsUnicode(false);
			entity.Property(e => e.SocialSecurityNumber)
				.IsRequired()
				.HasMaxLength(9)
				.IsUnicode(false);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
