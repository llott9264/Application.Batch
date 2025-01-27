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

			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Begin generating file for Customer List.", LogType.Information), cancellationToken);

			List<Customer> customers = unitOfWork.Customers.GetAll();

			if (customers.Count > 0)
			{
				if (outgoingFile.WriteFile(customers))
				{
					await outgoingFile.EncryptFile();

					if (outgoingFile.DoesArchiveGpgFileExist())
					{
						outgoingFile.MoveGpgFileToDataTransferFolder();
						await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Successfully completed workflow.", LogType.Information), cancellationToken);
						//await mediator.Send(new SendEmailCommand("", "", "", "", ";"), cancellationToken);

						await outgoingFile.MoveArchiveFileToProcessedFolder();
						await outgoingFile.MoveArchiveGpgFileToProcessFolder();
					}
					else
					{
						await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Failed to encrypt Customer file.", LogType.Error), cancellationToken);
						await outgoingFile.MoveArchiveFileToFailedFolder();
						await outgoingFile.MoveArchiveGpgFileToProcessFolder();
					}
				}
				else
				{
					await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Failed to write Customer file.", LogType.Error), cancellationToken);
				}
			}
			else
			{
				await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - No customers were found. File not generated.", LogType.Warning), cancellationToken);
			}
		}
		catch (Exception e)
		{
			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - Error occurred generating Customer List.  Error message: {e.Message}", LogType.Error), cancellationToken);
		}
		finally
		{
			await outgoingFile.CleanUpArchiveFolder();
			await mediator.Send(new CreateLogCommand($"{outgoingFile.BatchName} - End generating file for Customer List.", LogType.Information), cancellationToken);
		}
	}
}