using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Utilities.UnitOfWork.Contracts;

namespace Application.Batch.Infrastructure.Persistence;

public interface IDbContext : IDbContextBase
{
	public DbSet<Address> Addresses { get; set; }
	public DbSet<Customer> Customers { get; set; }
}
