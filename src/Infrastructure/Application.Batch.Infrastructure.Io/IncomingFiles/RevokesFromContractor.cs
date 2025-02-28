using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.VisualBasic.FileIO;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.IncomingFiles;

internal class RevokesFromContractor(IMediator mediator, IMapper mapper)
	: Bases.IncomingFiles(mediator,
		GetArchiveFolderBasePath(mediator),
		GetDataTransferFolderBasePath(mediator),
		GetGpgPrivateKeyName(mediator),
		GetGpgPrivateKeyPassword(mediator)), IRevokesFromContractor
{
	public string BatchName => "Revokes From Contractor";

	public List<RevokeViewModel> ReadFiles()
	{
		List<RevokeViewModel> revokes = new();

		try
		{
			foreach (DecryptionFileDto file in Files)
			{
				using (TextFieldParser reader = new(file.ArchiveFileFullPath))
				{
					reader.TextFieldType = FieldType.FixedWidth;
					int[] fieldWidths = [9, 1];
					reader.SetFieldWidths(fieldWidths);

					while (!reader.EndOfData)
					{
						string[]? line = reader.ReadFields();

						if (line != null)
						{
							revokes.Add(mapper.Map<RevokeViewModel>((line[0], line[1])));
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Mediator.Send(new CreateLogCommand($"{BatchName} - Error occurred reading file.  Error message: {e.Message}", LogType.Error));
		}

		return revokes;
	}

	private static string GetArchiveFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RevokesFromContractor:ArchivePath")).Result;
	}

	private static string GetDataTransferFolderBasePath(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RevokesFromContractor:DataTransferPath")).Result;
	}

	private static string GetGpgPrivateKeyName(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RevokesFromContractor:PrivateKeyName")).Result;
	}

	private static string GetGpgPrivateKeyPassword(IMediator mediator)
	{
		return mediator.Send(new GetConfigurationByKeyQuery("Workflows:RevokesFromContractor:PrivateKeyPassword")).Result;
	}
}