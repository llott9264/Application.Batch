using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Batch.Infrastructure.Persistence;

public interface IDbContext : IDbContextBase
{
	public DbSet<Address> Addresses { get; set; }
	public DbSet<Customer> Customers { get; set; }
}
