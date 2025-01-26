using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;


public abstract class FileBase(IMediator mediator, string archiveFolderBasePath, string dataTransferFolderBasePath) : IFileBase
{
	private readonly string _folderName = DateTime.Now.ToString("MMddyyyy");

	protected IMediator Mediator { get; } = mediator;
	public string ArchiveFolderBasePath { get; } = archiveFolderBasePath;
	public string DataTransferFolderBasePath { get; } = dataTransferFolderBasePath;
	public string ArchiveFolder => $"{ArchiveFolderBasePath}{_folderName}";
	public string ArchiveProcessedFolder => $@"{ArchiveFolder}\Processed\";
	public string ArchiveFailedFolder => $@"{ArchiveFolder}\Failed\";

	public void CleanUpArchiveFolder()
	{
		int fileRetentionLengthInMonths = Convert.ToInt32(Mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths")));
		Mediator.Send(new CreateLogCommand($"Begin cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
		Utilities.IoOperations.Directory.DeleteDirectory(new DirectoryInfo(ArchiveFolderBasePath), fileRetentionLengthInMonths, true);
		Mediator.Send(new CreateLogCommand($"End cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
	}

	public void MoveToFolder(string sourceFile, string destinationFolder)
	{
		if (Utilities.IoOperations.File.Move(sourceFile, destinationFolder))
		{
			Mediator.Send(new CreateLogCommand($"File was successfully moved to destination folder.", LogType.Information));
		}
	}

	public void CreateArchiveDirectory()
	{
		Utilities.IoOperations.Directory.CreateDirectory(ArchiveFolder);
	}

	public void DeleteFilesInDataTransferFolder()
	{
		Utilities.IoOperations.Directory.DeleteFilesInFolder(new DirectoryInfo(DataTransferFolderBasePath));
	}
}