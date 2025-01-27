using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.Bases;
using MediatR;

namespace Application.Batch.Infrastructure.Io.OutgoingFiles;

public class RenewalsToPrintContractor(IMediator mediator) : OutgoingFile(mediator, GetArchiveFolderBasePath(mediator), GetDataTransferFolderBasePath(mediator),
	"RenewalCustomerList.txt", "RenewalCustomerList.txt.gpg", GetGpgPublicKeyName(mediator)), IRenewalsToPrintContractor
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
			Mediator.Send(new CreateLogCommand($"{BatchName} - Error occurred writing file.  Error message: {e.Message}", LogType.Error));
		}

		return isSuccessful;
	}

	private static string GetArchiveFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RenewalsToPrintContractor:ArchivePath")).Result;
	}

	private static string GetDataTransferFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RenewalsToPrintContractor:DataTransferPath")).Result;
	}

	private static string GetGpgPublicKeyName(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RenewalsToPrintContractor:PublicKey")).Result;
	}
}