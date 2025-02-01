using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Domain.Entities;
using MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.RenewalsToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, IRenewalsToPrintContractor outgoingFile, IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			outgoingFile.CreateArchiveDirectory();
			outgoingFile.DeleteFilesInDataTransferFolder();

			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Begin generating file for Renewals To Print Contractor.", LogType.Information), cancellationToken);

			List<Customer> customers = unitOfWork.Customers.GetCustomersIncludeAddresses();

			if (customers.Count == 0)
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - No customers were found. Files not generated.", LogType.Warning), cancellationToken);
				return;
			}

			if (!outgoingFile.WriteFiles(customers))
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Failed to write files for Renewals To Print Contractor.", LogType.Error), cancellationToken);
				return;
			}

			await outgoingFile.EncryptFiles();
			
			if (outgoingFile.DoArchiveGpgFilesExist())
			{
				await outgoingFile.MoveGpgFilesToDataTransferFolder();
				await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Successfully completed workflow.", LogType.Information), cancellationToken);
				//await mediator.Send(new SendEmailCommand("", "", "", "", ";"), cancellationToken);

				outgoingFile.MoveArchiveFilesToProcessedFolder();
				outgoingFile.MoveArchiveGpgFilesToProcessFolder();
			}
			else
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Failed to encrypt file for Renewals To Print Contractor.", LogType.Error), cancellationToken);
				outgoingFile.MoveArchiveFilesToFailedFolder();
				outgoingFile.MoveArchiveGpgFilesToFailedFolder();
			}
		}
		catch (Exception e)
		{
			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Error occurred generating file for Renewals To Print Contractor.  Error message: {e.Message}", LogType.Error), cancellationToken);
		}
		finally
		{
			await outgoingFile.CleanUpArchiveFolder();
			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - End generating file for Customer List.", LogType.Information), cancellationToken);
		}
	}
}