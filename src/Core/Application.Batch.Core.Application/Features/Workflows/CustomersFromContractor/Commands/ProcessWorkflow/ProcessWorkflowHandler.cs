using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, ICustomersFromContractor incomingFile, IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			incomingFile.CreateArchiveDirectory();
			await incomingFile.MoveToGpgFileToArchiveFolder();

			await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - Begin generating file for Customer List.", LogType.Information), cancellationToken);
			
			if (incomingFile.DoesArchiveGpgFileExist())
			{
				await incomingFile.DecryptFile();

				if (incomingFile.DoesArchiveFileExist())
				{
					List<Customer> customers = incomingFile.ReadFile();
					unitOfWork.Customers.AddRange(customers);
					unitOfWork.Complete();

					await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - Successfully imported customer data.", LogType.Information), cancellationToken);
				}
				else
				{
					await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - Failed to decrypt file.", LogType.Error), cancellationToken);
				}
			}
			else
			{
				await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - Failed to move gpg file to archive folder.", LogType.Error), cancellationToken);
			}
		}
		catch (Exception e)
		{
			await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - Error occurred generating Customer List.  Error message: {e.Message}", LogType.Error), cancellationToken);
		}
		finally
		{
			await incomingFile.CleanUpArchiveFolder();
			await mediator.Send(new CreateLogCommand($"{incomingFile.BatchName} - End generating file for Customer List.", LogType.Information), cancellationToken);
		}
	}
}
