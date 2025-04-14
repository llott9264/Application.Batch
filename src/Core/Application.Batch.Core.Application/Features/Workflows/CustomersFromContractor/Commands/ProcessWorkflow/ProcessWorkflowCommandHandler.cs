using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowCommandHandler(
	IMediator mediator,
	ICustomersFromContractor incomingFile,
	IUnitOfWork unitOfWork) : IRequestHandler<ProcessWorkflowCommand>
{
	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			await incomingFile.CreateArchiveDirectory();
			await incomingFile.MoveToGpgFileToArchiveFolder();

			await mediator.Send(
				new CreateLogCommand($"{incomingFile.BatchName} - Begin generating file for Customer List.",
					LogType.Information), cancellationToken);

			if (incomingFile.DoesArchiveGpgFileExist())
			{
				await incomingFile.DecryptFile();

				if (incomingFile.DoesArchiveFileExist())
				{
					List<CustomerViewModel> customers = await incomingFile.ReadFile();


					foreach (CustomerViewModel customerViewModel in customers)
					{
						Customer? customer = unitOfWork.Customers
							.Find(c => c.SocialSecurityNumber == customerViewModel.SocialSecurityNumber)
							.FirstOrDefault();

						if (customer != null)
						{
							customer.FirstName = customerViewModel.FirstName;
							customer.LastName = customerViewModel.LastName;
						}
						else
						{
							unitOfWork.Customers.Add(new Customer
							{
								FirstName = customerViewModel.FirstName,
								LastName = customerViewModel.LastName,
								SocialSecurityNumber = customerViewModel.SocialSecurityNumber
							});
						}
					}

					unitOfWork.Complete();
					await mediator.Send(
						new CreateLogCommand($"{incomingFile.BatchName} - Successfully imported customer data.",
							LogType.Information), cancellationToken);
					//await mediator.Send(new SendEmailCommand("", "", "", "", ";"), cancellationToken);

					await incomingFile.MoveArchiveFileToProcessedFolder();
					await incomingFile.MoveArchiveGpgFileToProcessedFolder();
				}
				else
				{
					await mediator.Send(
						new CreateLogCommand($"{incomingFile.BatchName} - Failed to decrypt file.", LogType.Error),
						cancellationToken);
					await incomingFile.MoveArchiveGpgFileToFailedFolder();
				}
			}
			else
			{
				await mediator.Send(
					new CreateLogCommand($"{incomingFile.BatchName} - Failed to move gpg file to archive folder.",
						LogType.Error), cancellationToken);
			}
		}
		catch (Exception e)
		{
			await mediator.Send(
				new CreateLogCommand(
					$"{incomingFile.BatchName} - Error occurred generating Customer List.  Error message: {e.Message}",
					LogType.Error), cancellationToken);
		}
		finally
		{
			int fileRetentionLengthInMonths =
				Convert.ToInt32(await mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths"),
					cancellationToken));
			await incomingFile.CleanUpArchiveFolder(fileRetentionLengthInMonths);
			await mediator.Send(
				new CreateLogCommand($"{incomingFile.BatchName} - End generating file for Customer List.",
					LogType.Information), cancellationToken);
		}
	}
}
