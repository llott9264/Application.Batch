using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Pdf;
using Application.Batch.Core.Domain.Entities;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.OutgoingFiles;

internal class RenewalsToPrintContractor(IMediator mediator, IRenewalsToPrintContractorPdf pdf)
	: Bases.OutgoingFiles(mediator,
		GetArchiveFolderBasePath(mediator),
		GetDataTransferFolderBasePath(mediator),
		GetGpgPublicKeyName(mediator)), IRenewalsToPrintContractor
{
	public string BatchName => "Renewals To Print Contractor";

	public bool WriteFiles(List<Customer> customers)
	{
		bool isSuccessful = false;

		try
		{
			string pdfTemplateFullPath = Mediator.Send(new GetConfigurationByKeyQuery("Workflows:RenewalsToPrintContractor:PdfTemplatePath")).Result;
			int documentsPerFile = Convert.ToInt32(Mediator.Send(new GetConfigurationByKeyQuery("Workflows:RenewalsToPrintContractor:DocumentsPerFile")).Result);

			List<Customer[]> result = customers.Chunk(documentsPerFile).ToList();
			string fileNameBase = "Renewals_";
			int fileCount = 1;

			foreach (Customer[] chunk in result)
			{
				string archiveFileFullPath = GetArchiveFileFullPath($"{fileNameBase}{fileCount}.pdf");
				pdf.CreatePdf(pdfTemplateFullPath, archiveFileFullPath, chunk);
				AddFileToEncrypt(Path.GetFileName(archiveFileFullPath));
				fileCount++;
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