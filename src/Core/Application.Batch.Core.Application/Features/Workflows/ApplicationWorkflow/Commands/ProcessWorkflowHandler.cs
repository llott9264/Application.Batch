using Application.Batch.Core.Application.Enums;
using MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;

public class ProcessWorkflowHandler(IMediator mediator) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		switch (request.WorkflowName)
		{
			case WorkflowName.CustomersToPrintContractor:
				await mediator.Send(new CustomersToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand(), cancellationToken);
				break;
			case WorkflowName.RenewalsToPrintContractor:
				await mediator.Send(new RenewalsToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand(), cancellationToken);
				break;
			case WorkflowName.CustomersFromContractor:
				await mediator.Send(new CustomersFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand(), cancellationToken);
				break;
			case WorkflowName.RevokesFromContractor:
				await mediator.Send(new RevokesFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand(), cancellationToken);
				break;
			default:
				await mediator.Send(new CreateLogCommand($"Invalid parameter: {request.WorkflowName}", LogType.Error), cancellationToken);
				break;
		}
	}
}