namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface IUnitOfWorkBase : IDisposable
{
	int Complete();
	Task<int> CompleteAsync();
	int Complete(int commandTimeoutInSeconds);
	Task<int> CompleteAsync(int commandTimeoutInSeconds);
}
