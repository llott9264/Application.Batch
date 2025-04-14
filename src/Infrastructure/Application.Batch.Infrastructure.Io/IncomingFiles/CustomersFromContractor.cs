using System.Runtime.CompilerServices;
using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using AutoMapper;
using MediatR;
using Microsoft.VisualBasic.FileIO;
using Utilities.Configuration.MediatR;
using Utilities.FileManagement.Infrastructure;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

[assembly: InternalsVisibleTo("Application.Batch.Infrastructure.Io.Tests")]

namespace Application.Batch.Infrastructure.Io.IncomingFiles;

internal class CustomersFromContractor(IMediator mediator, IMapper mapper) : IncomingFile(mediator,
		GetArchiveFolderBasePath(mediator), GetDataTransferFolderBasePath(mediator),
		GetGpgPrivateKeyName(mediator), GetGpgPrivateKeyPassword(mediator), "CustomerList.txt", "CustomerList.txt.gpg"),
	ICustomersFromContractor
{
	public string BatchName => "Customers From Print Contractor";

	public async Task<List<CustomerViewModel>> ReadFile()
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
			await Mediator.Send(new CreateLogCommand(
				$"{BatchName} - Error occurred reading file.  Error message: {e.Message}", LogType.Error));
		}

		return customers;
	}

	private static string GetArchiveFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:ArchivePath")).Result;
	}

	private static string GetDataTransferFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:DataTransferPath"))
			.Result;
	}

	private static string GetGpgPrivateKeyName(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:PrivateKeyName")).Result;
	}

	private static string GetGpgPrivateKeyPassword(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:CustomersFromContractor:PrivateKeyPassword"))
			.Result;
	}
}
