using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Infrastructure.Persistence.Repositories;

namespace Application.Batch.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
	private readonly ApplicationDbContext _context;

	public UnitOfWork(ApplicationDbContext context)
	{
		_context = context;
		Customer = new CustomerRepository(_context);
		Address = new AddressRepository(_context);
	}

	public ICustomerRepository Customer { get; }
	public IAddressRepository Address { get; }
	public int Complete()
	{
		return _context.SaveChanges();
	}

	public void Dispose()
	{
		_context.Dispose();
		GC.SuppressFinalize(this);
	}
}