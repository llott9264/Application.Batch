using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.CustomersToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowCommandHandler(
	IMediator mediator,
	ICustomersToPrintContractor outgoingFile,
	IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await outgoingFile.CreateArchiveDirectory();
			await outgoingFile.DeleteFilesInDataTransferFolder();

			await mediator.Send(
				new CreateLogCommand($"{outgoingFile.BatchName} - Begin generating file for Customer List.",
					LogType.Information), cancellationToken);

			List<Customer> customers = unitOfWork.Customers.GetAll();

			if (customers.Count == 0)
			{
				await mediator.Send(
					new CreateLogCommand($"{outgoingFile.BatchName} - No customers were found. File not generated.",
						LogType.Warning), cancellationToken);
				return;
			}

			if (!outgoingFile.WriteFile(customers))
			{
				await mediator.Send(
					new CreateLogCommand($"{outgoingFile.BatchName} - Failed to write Customer file.", LogType.Error),
					cancellationToken);
				return;
			}

			await outgoingFile.EncryptFile();

			if (outgoingFile.DoesArchiveGpgFileExist())
			{
				await outgoingFile.CopyGpgFileToDataTransferFolder();
				await mediator.Send(
					new CreateLogCommand($"{outgoingFile.BatchName} - Successfully completed workflow.",
						LogType.Information), cancellationToken);
				//await mediator.Send(new SendEmailCommand("", "", "", "", ";"), cancellationToken);

				await outgoingFile.MoveArchiveFileToProcessedFolder();
				await outgoingFile.MoveArchiveGpgFileToProcessedFolder();
			}
			else
			{
				await mediator.Send(
					new CreateLogCommand($"{outgoingFile.BatchName} - Failed to encrypt Customer file.", LogType.Error),
					cancellationToken);
				await outgoingFile.MoveArchiveFileToFailedFolder();
			}
		}
		catch (Exception e)
		{
			await mediator.Send(
				new CreateLogCommand(
					$"{outgoingFile.BatchName} - Error occurred generating Customer List.  Error message: {e.Message}",
					LogType.Error), cancellationToken);
		}
		finally
		{
			int fileRetentionLengthInMonths =
				Convert.ToInt32(await mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths"),
					cancellationToken));
			await outgoingFile.CleanUpArchiveFolder(fileRetentionLengthInMonths);
			await mediator.Send(
				new CreateLogCommand($"{outgoingFile.BatchName} - End generating file for Customer List.",
					LogType.Information), cancellationToken);
		}
	}
}
