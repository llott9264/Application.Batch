namespace Application.Batch.Infrastructure.Persistence;

public interface IDatabaseFacadeWrapper
{
	int? GetCommandTimeout();
	void SetCommandTimeout(int? timeout);
}
