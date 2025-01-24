using Application.Batch.Core.Application.Contracts.Presentation;
using Utilities.Logging.EventLog;

namespace Application.Batch.Infrastructure.Common;

public class Application(ILog logger) : IApplication
{
	public void Run(string workFlowName)
	{
		switch (workFlowName)
		{
			case "CustomersToPrintContractor":
				CustomersToPrintContractor.Run();
				break;
			case "RenewalsToPrintContractor":
				//workflowManager.RenewalsToPrintContractor.Run();
				break;
			default:
				logger.Error($"Invalid parameter: {workFlowName}");
				break;
		}
	}
}