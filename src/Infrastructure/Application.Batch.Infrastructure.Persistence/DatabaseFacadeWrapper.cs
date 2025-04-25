using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Batch.Infrastructure.Persistence;

public class DatabaseFacadeWrapper(DatabaseFacade databaseFacade) : IDatabaseFacadeWrapper
{
	public int? GetCommandTimeout()
	{
		return databaseFacade.GetCommandTimeout();
	}

	public void SetCommandTimeout(int? timeout)
	{
		databaseFacade.SetCommandTimeout(timeout);
	}
}
