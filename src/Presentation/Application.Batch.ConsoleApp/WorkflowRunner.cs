using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;
using MediatR;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.ConsoleApp;

public class WorkflowRunner(IMediator? mediator)
{
	public async Task RunAsync(string[] args)
	{
		if (mediator == null)
		{
			throw new ArgumentNullException(nameof(mediator));
		}

		Environment.SetEnvironmentVariable("ITEXT_BOUNCY_CASTLE_FACTORY_NAME", "bouncy-castle");
		string workFlowName = args.Length > 0 ? args[0] : "default";

		if (Enum.TryParse(workFlowName, out WorkflowName workflowEnum))
		{
			await mediator.Send(new ProcessWorkflowCommand(workflowEnum));
		}
		else
		{
			await mediator.Send(new CreateLogCommand($"Invalid parameter: {workFlowName}", LogType.Error));
		}
	}
}