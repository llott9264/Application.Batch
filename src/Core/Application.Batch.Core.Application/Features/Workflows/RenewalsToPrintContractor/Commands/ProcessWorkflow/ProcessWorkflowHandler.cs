using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.RenewalsToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, IRenewalsToPrintContractor outgoingFiles, IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await outgoingFiles.CreateArchiveDirectory();
			await outgoingFiles.DeleteFilesInDataTransferFolder();

			await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Begin generating file for Renewals To Print Contractor.", LogType.Information), cancellationToken);

			List<Customer> customers = unitOfWork.Customers.GetCustomersIncludeAddresses();

			if (customers.Count == 0)
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - No customers were found. Files not generated.", LogType.Warning), cancellationToken);
				return;
			}

			if (!outgoingFiles.WriteFiles(customers))
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Failed to write files for Renewals To Print Contractor.", LogType.Error), cancellationToken);
				return;
			}

			if (!outgoingFiles.DoArchiveFilesExist())
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Failed to find files for Renewals To Print Contractor.", LogType.Error), cancellationToken);
				return;
			}

			await outgoingFiles.EncryptFiles();
			
			if (outgoingFiles.DoArchiveGpgFilesExist())
			{
				await outgoingFiles.CopyGpgFilesToDataTransferFolder();
				await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Successfully completed workflow.", LogType.Information), cancellationToken);
				//await mediator.Send(new SendEmailCommand("", "", "", "", ";"), cancellationToken);

				await outgoingFiles.MoveArchiveFilesToProcessedFolder();
				await outgoingFiles.MoveArchiveGpgFilesToProcessedFolder();
			}
			else
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Failed to encrypt file for Renewals To Print Contractor.", LogType.Error), cancellationToken);
				await outgoingFiles.MoveArchiveFilesToFailedFolder();
				await outgoingFiles.MoveArchiveGpgFilesToFailedFolder();
			}
		}
		catch (Exception e)
		{
			await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - Error occurred generating file for Renewals To Print Contractor.  Error message: {e.Message}", LogType.Error), cancellationToken);
		}
		finally
		{
			await outgoingFiles.CleanUpArchiveFolder();
			await mediator.Send(new CreateLogCommand($"{outgoingFiles.BatchName} - End generating file for Customer List.", LogType.Information), cancellationToken);
		}
	}
}