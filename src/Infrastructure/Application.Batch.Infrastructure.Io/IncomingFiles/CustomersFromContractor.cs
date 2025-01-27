using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.Bases;
using MediatR;
using Microsoft.VisualBasic.FileIO;

namespace Application.Batch.Infrastructure.Io.IncomingFiles;

public class CustomersFromContractor(IMediator mediator) : IncomingFile(mediator, GetArchiveFolderBasePath(mediator), GetDataTransferFolderBasePath(mediator),
	"CustomerList.txt", "CustomerList.txt.gpg", GetGpgPrivateKeyName(mediator), GetGpgPrivateKeyPassword(mediator)), ICustomersFromContractor
{
	public string BatchName => "Customers From Print Contractor";

	public List<Customer> ReadFile()
	{
		List<Customer> customers = new();

		try
		{
			using (TextFieldParser reader = new(ArchiveFileFullPath))
			{
				reader.TextFieldType = FieldType.FixedWidth;
				int[] fieldWidths = [100, 100, 9];
				reader.SetFieldWidths(fieldWidths);

				while (!reader.EndOfData)
				{
					string[] line = reader.ReadFields();

					customers.Add(new Customer()
					{
						FirstName = line[0],
						LastName = line[1],
						SocialSecurityNumber = line[2]
					});
				}
			}
		}
		catch (Exception e)
		{
			Mediator.Send(new CreateLogCommand($"{BatchName} - Error occurred writing file.  Error message: {e.Message}", LogType.Error));
		}

		return customers;
	}

	private static string GetArchiveFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:ArchivePath")).Result;
	}

	private static string GetDataTransferFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:DataTransferPath")).Result;
	}

	private static string GetGpgPrivateKeyName(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:PrivateKey")).Result;
	}

	private static string GetGpgPrivateKeyPassword(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:PrivateKeyPassword")).Result;
	}
}