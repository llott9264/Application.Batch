using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.FileManagement.Infrastructure;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.OutgoingFiles;

internal class CustomersToPrintContractor(IMediator mediator)
	: OutgoingFile(mediator,
		GetArchiveFolderBasePath(mediator),
		GetDataTransferFolderBasePath(mediator),
		"CustomerList.txt",
		"CustomerList.txt.gpg", GetGpgPublicKeyName(mediator)), ICustomerToPrintContractor
{
	public string BatchName => "Customer To Print Contractor";

	public bool WriteFile(List<Customer> customers)
	{
		bool isSuccessful = false;

		try
		{
			using (StreamWriter writer = new(ArchiveFileFullPath))
			{
				foreach (Customer customer in customers)
				{
					writer.WriteLine($"{customer.LastName},{customer.FirstName}{customer.SocialSecurityNumber}");
					writer.Flush();
				}
			}

			isSuccessful = true;
		}
		catch (Exception e)
		{
			Mediator.Send(new CreateLogCommand(
				$"{BatchName} - Error occurred writing file.  Error message: {e.Message}", LogType.Error));
		}

		return isSuccessful;
	}

	private static string GetArchiveFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersToPrintContractor:ArchivePath")).Result;
	}

	private static string GetDataTransferFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersToPrintContractor:DataTransferPath"))
			.Result;
	}

	private static string GetGpgPublicKeyName(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersToPrintContractor:PublicKey")).Result;
	}
}
