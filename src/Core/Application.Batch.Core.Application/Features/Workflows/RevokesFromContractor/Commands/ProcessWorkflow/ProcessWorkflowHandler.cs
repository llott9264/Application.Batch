using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Domain.Entities;
using MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, IRevokesFromContractor incomingFiles, IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - Begin revoking Customer List.", LogType.Information), cancellationToken);

			incomingFiles.CreateArchiveDirectory();
			incomingFiles.GetGpgFilesInDataTransferFolder();
			await incomingFiles.MoveGpgFilesToArchiveFolder();

			if (incomingFiles.DoArchiveGpgFilesExist())
			{
				await incomingFiles.DecryptFiles();

				if (incomingFiles.DoArchiveFilesExist())
				{
					List<RevokeViewModel> revokes = incomingFiles.ReadFiles();

					foreach (RevokeViewModel revokeViewModel in revokes)
					{
						Customer? customer = unitOfWork.Customers.Find(c => c.SocialSecurityNumber == revokeViewModel.SocialSecurityNumber).FirstOrDefault();

						if (customer != null)
						{
							customer.IsRevoked = revokeViewModel.IsRevoked;
						}
					}

					unitOfWork.Complete();

					await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - Successfully imported customer data.", LogType.Information), cancellationToken);
				}
				else
				{
					await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - Failed to decrypt files.", LogType.Error), cancellationToken);
				}
			}
			else
			{
				await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - Failed to move gpg files to archive folder.", LogType.Error), cancellationToken);
			}
		}
		catch (Exception e)
		{
			await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - Error occurred revoking customer.  Error message: {e.Message}", LogType.Error), cancellationToken);
		}
		finally
		{
			await incomingFiles.CleanUpArchiveFolder();
			await mediator.Send(new CreateLogCommand($"{incomingFiles.BatchName} - End revoking customer.", LogType.Information), cancellationToken);
		}
	}
}