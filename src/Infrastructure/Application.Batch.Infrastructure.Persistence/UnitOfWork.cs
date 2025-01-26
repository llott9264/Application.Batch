using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Batch.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
	private readonly IDbContext _context;

	public UnitOfWork(IDbContext context)
	{
		_context = context;
		Customers = new CustomerRepository(_context);
		Addresses = new AddressRepository(_context);
	}

	public ICustomerRepository Customers { get; }
	public IAddressRepository Addresses { get; }
	public int Complete()
	{
		return _context.SaveChanges();
	}

	public async Task<int> CompleteAsync()
	{
		return await _context.SaveChangesAsync();
	}

	public int Complete(int commandTimeoutInSeconds)
	{
		int? originalCommandTimeout = _context.Database.GetCommandTimeout();
		_context.Database.SetCommandTimeout(commandTimeoutInSeconds);

		int numberOfRecordsChanged = _context.SaveChanges();
		_context.Database.SetCommandTimeout(originalCommandTimeout);

		return numberOfRecordsChanged;
	}

	public async Task<int> CompleteAsync(int commandTimeoutInSeconds)
	{
		int? originalCommandTimeout = _context.Database.GetCommandTimeout();
		_context.Database.SetCommandTimeout(commandTimeoutInSeconds);

		int numberOfRecordsChanged = await _context.SaveChangesAsync();
		_context.Database.SetCommandTimeout(originalCommandTimeout);

		return numberOfRecordsChanged;
	}

	public void Dispose()
	{
		_context.Dispose();
		GC.SuppressFinalize(this);
	}
}