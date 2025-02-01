using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Infrastructure.Io.Bases;
using AutoMapper;
using MediatR;
using Microsoft.VisualBasic.FileIO;

namespace Application.Batch.Infrastructure.Io.IncomingFiles;

public class CustomersFromContractor(IMediator mediator, IMapper mapper) : IncomingFile(mediator, GetArchiveFolderBasePath(mediator), GetDataTransferFolderBasePath(mediator),
	"CustomerList.txt", "CustomerList.txt.gpg", GetGpgPrivateKeyName(mediator), GetGpgPrivateKeyPassword(mediator)), ICustomersFromContractor
{
	public string BatchName => "Customers From Print Contractor";

	public List<CustomerViewModel> ReadFile()
	{
		List<CustomerViewModel> customers = new();

		try
		{
			using (TextFieldParser reader = new(ArchiveFileFullPath))
			{
				reader.TextFieldType = FieldType.FixedWidth;
				int[] fieldWidths = [100, 100, 9];
				reader.SetFieldWidths(fieldWidths);

				while (!reader.EndOfData)
				{
					string[]? line = reader.ReadFields();

					if (line != null)
					{
						customers.Add(mapper.Map<CustomerViewModel>((line[0], line[1], line[2])));
					}
				}
			}
		}
		catch (Exception e)
		{
			Mediator.Send(new CreateLogCommand($"{BatchName} - Error occurred reading file.  Error message: {e.Message}", LogType.Error));
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