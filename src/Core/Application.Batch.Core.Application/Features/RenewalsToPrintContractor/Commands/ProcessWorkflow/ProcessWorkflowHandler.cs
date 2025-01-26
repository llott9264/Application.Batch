using Application.Batch.Core.Application.Contracts.Persistence;
using AutoMapper;
using MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application.Features.RenewalsToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMapper mapper, IUnitOfWork unitOfWork, ILog logger) : IRequestHandler<ProcessWorkflowCommand>
{
	public Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}