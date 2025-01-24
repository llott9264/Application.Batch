namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface IUnitOfWork
{
	ICustomerRepository Customer { get; }
	IAddressRepository Address { get; }
	int Complete();
}