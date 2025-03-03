using Application.Batch.Core.Application.Contracts.Io;
using MediatR;
using Utilities.Configuration.MediatR;
using Utilities.IoOperations.MediatR.Directory.CleanUpDirectory;
using Utilities.IoOperations.MediatR.Directory.CreateDirectory;
using Utilities.IoOperations.MediatR.Directory.DeleteFiles;
using Utilities.IoOperations.MediatR.File.MoveFile;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;


public abstract class FileBase(IMediator mediator, string archiveFolderBasePath, string dataTransferFolderBasePath) : IFileBase
{
	private readonly string _folderName = DateTime.Now.ToString("MMddyyyy");

	protected IMediator Mediator { get; } = mediator;
	public string ArchiveFolderBasePath { get; } = archiveFolderBasePath;
	public string DataTransferFolderBasePath { get; } = dataTransferFolderBasePath;
	public string ArchiveFolder => @$"{ArchiveFolderBasePath}{_folderName}\";
	public string ArchiveProcessedFolder => $@"{ArchiveFolder}Processed\";
	public string ArchiveFailedFolder => $@"{ArchiveFolder}Failed\";

	public async Task CleanUpArchiveFolder()
	{
		int fileRetentionLengthInMonths = Convert.ToInt32(await Mediator.Send(new GetConfigurationByKeyQuery("FileRetentionPeriodInMonths")));
		await Mediator.Send(new CreateLogCommand($"Begin cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
		await Mediator.Send(new CleanUpDirectoryCommand(new DirectoryInfo(ArchiveFolderBasePath), fileRetentionLengthInMonths, true), CancellationToken.None);
		await Mediator.Send(new CreateLogCommand($"End cleaning up of the Archive folder:  {ArchiveFolderBasePath}", LogType.Information));
	}

	public async Task MoveToFolder(string sourceFile, string destinationFolder)
	{
		await Mediator.Send(new MoveFileCommand(sourceFile, destinationFolder), CancellationToken.None);
	}

	public async Task CreateArchiveDirectory()
	{
		await Mediator.Send(new CreateDirectoryCommand(ArchiveFolder), CancellationToken.None);
	}

	public async Task DeleteFilesInDataTransferFolder()
	{
		await Mediator.Send(new DeleteFilesCommand(new DirectoryInfo(DataTransferFolderBasePath)), CancellationToken.None);
	}
}