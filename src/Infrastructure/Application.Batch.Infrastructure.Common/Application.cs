using Application.Batch.Core.Application.Contracts.Presentation;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using MediatR;
using CustomersToPrintContractor = Application.Batch.Core.Application.Features.Workflows.CustomersToPrintContractor;
using RenewalsToPrintContractor = Application.Batch.Core.Application.Features.Workflows.RenewalsToPrintContractor;

namespace Application.Batch.Infrastructure.Common;

public class Application(IMediator mediator) : IApplication
{
	public void Run(string workFlowName)
	{
		switch (workFlowName)
		{
			case "CustomersToPrintContractor":
				mediator.Send(new CustomersToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand());
				break;
			case "RenewalsToPrintContractor":
				mediator.Send(new RenewalsToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand());
				break;
			default:
				mediator.Send(new CreateLogCommand($"Invalid parameter: {workFlowName}", LogType.Error));
				break;
		}
	}
}