using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowCommandHandler(
	IMediator mediator,
	IRevokesFromContractor incomingFiles,
	IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await mediator.Send(
				new CreateLogCommand($"{incomingFiles.BatchName} - Begin revoking Customer List.", LogType.Information),
				cancellationToken);
			incomingFiles.GetGpgFilesInDataTransferFolder();

			if (!incomingFiles.Files.Any())
			{
				await mediator.Send(
					new CreateLogCommand($"{incomingFiles.BatchName} - No files to decrypt.", LogType.Warning),
					cancellationToken);
				return;
			}

			await incomingFiles.CreateArchiveDirectory();
			await incomingFiles.MoveGpgFilesToArchiveFolder();

			if (!incomingFiles.DoArchiveGpgFilesExist())
			{
				await mediator.Send(
					new CreateLogCommand($"{incomingFiles.BatchName} - Failed to move gpg files to archive folder.",
						LogType.Error), cancellationToken);
				return;
			}

			await incomingFiles.DecryptFiles();

			if (!incomingFiles.DoArchiveFilesExist())
			{
				await mediator.Send(
					new CreateLogCommand($"{incomingFiles.BatchName} - Failed to decrypt files.", LogType.Error),
					cancellationToken);
				return;
			}

			List<RevokeViewModel> revokes = incomingFiles.ReadFiles();

			foreach (RevokeViewModel revokeViewModel in revokes)
			{
				Customer? customer = unitOfWork.Customers
					.Find(c => c.SocialSecurityNumber == revokeViewModel.SocialSecurityNumber).FirstOrDefault();

				if (customer != null)
				{
					customer.IsRevoked = revokeViewModel.IsRevoked;
				}
			}

			unitOfWork.Complete();

			await mediator.Send(
				new CreateLogCommand($"{incomingFiles.BatchName} - Successfully imported revokes data.",
					LogType.Information), cancellationToken);
		}
		catch (Exception e)
		{
			await mediator.Send(
				new CreateLogCommand(
					$"{incomingFiles.BatchName} - Error occurred revoking customer.  Error message: {e.Message}",
					LogType.Error), cancellationToken);
		}
		finally
		{
			int fileRetentionLengthInMonths =
				Convert.ToInt32(await mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths"),
					cancellationToken));
			await incomingFiles.CleanUpArchiveFolder(fileRetentionLengthInMonths);
			await mediator.Send(
				new CreateLogCommand($"{incomingFiles.BatchName} - End revoking customer.", LogType.Information),
				cancellationToken);
		}
	}
}
