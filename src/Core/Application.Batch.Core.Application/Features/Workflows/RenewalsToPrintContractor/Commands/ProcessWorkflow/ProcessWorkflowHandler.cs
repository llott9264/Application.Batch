using Application.Batch.Core.Application.Contracts.Persistence;
using MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.RenewalsToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}