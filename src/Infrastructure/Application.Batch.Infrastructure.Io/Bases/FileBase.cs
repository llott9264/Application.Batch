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

	public async Task CleanUpArchiveFolder()
	{
		int fileRetentionLengthInMonths = Convert.ToInt32(await Mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths")));
		await Mediator.Send(new CreateLogCommand($"Begin cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
		Utilities.IoOperations.Directory.DeleteDirectory(new DirectoryInfo(ArchiveFolderBasePath), fileRetentionLengthInMonths, true);
		await Mediator.Send(new CreateLogCommand($"End cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
	}

	public void MoveToFolder(string sourceFile, string destinationFolder)
	{
		Utilities.IoOperations.File.Move(sourceFile, destinationFolder);
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