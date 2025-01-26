namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface IUnitOfWork
{
	ICustomerRepository Customers { get; }
	IAddressRepository Addresses { get; }
	int Complete();
	Task<int> CompleteAsync();
	int Complete(int commandTimeoutInSeconds);
	Task<int> CompleteAsync(int commandTimeoutInSeconds);
}