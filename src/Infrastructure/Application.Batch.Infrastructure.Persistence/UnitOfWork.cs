using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Infrastructure.Persistence.Repositories;

namespace Application.Batch.Infrastructure.Persistence;

public class UnitOfWork(IDbContext context) : UnitOfWorkBase(context), IUnitOfWork
{
	public ICustomerRepository Customers { get; } = new CustomerRepository(context);
	public IAddressRepository Addresses { get; } = new AddressRepository(context);
}
