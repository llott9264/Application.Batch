using System.Linq.Expressions;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Gpg.Commands;
using Application.Batch.Core.Application.Features.Log.Commands;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Utilities.Email;

namespace Application.Batch.Core.Application.Features.CustomersToPrintContractor.Commands.ProcessWorkflow;

public class ProcessWorkflowHandler(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork, IEmail email) : IRequestHandler<ProcessWorkflowCommand>
{
	private const string BatchName = "Customer To Print Contractor";

	public async Task Handle(ProcessWorkflowCommand request, CancellationToken cancellationToken)
	{
		try
		{
			//string archiveFileFullPath = await mediator.Send(new GetConfigurationByKeyQuery(""));
			await mediator.Send(new CreateLogCommand($"{BatchName} - Begin generating file for Customer List.", LogType.Information));
			
			List<Customer> customers = mapper.Map<List<Customer>>(await unitOfWork.Customer.GetAllAsync());

			if (customers.Count > 0)
			{
				if (WriteFile(customers))
				{
					EncryptFile();

					if (File.Exists(ArchiveGpgFileFullPath))
					{
						MoveGpgFileToDataTransferFolder();
						mediator.Send(new CreateLogCommand($"{BatchName} - Successfully completed workflow.", LogType.Information));

						MoveToFolder(ArchiveFileFullPath, ArchiveProcessedFolder);
						MoveToFolder(ArchiveGpgFileFullPath, ArchiveProcessedFolder);
					}
					else
					{
						mediator.Send(new CreateLogCommand($"{BatchName} - Failed to encrypt Customer file.", LogType.Error));
						MoveToFolder(ArchiveFileFullPath, ArchiveFailedFolder);
					}
				}
				else
				{
					mediator.Send(new CreateLogCommand($"{BatchName} - Failed to write Customer file.", LogType.Error));
				}
			}
			else
			{
				mediator.Send(new CreateLogCommand($"{BatchName} - No customers were found. File not generated.", LogType.Error));
			}
		}
		catch (Exception e)
		{
			mediator.Send(new CreateLogCommand($"{BatchName} - Error occurred generating Customer List.  Error message: {e.Message}", LogType.Error));
		}

		CleanUpArchiveFolder();
		mediator.Send(new CreateLogCommand($"{BatchName} - End generating file for Customer List.", LogType.Information));
	}
}
