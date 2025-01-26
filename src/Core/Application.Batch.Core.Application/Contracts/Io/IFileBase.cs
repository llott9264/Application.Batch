namespace Application.Batch.Core.Application.Contracts.Io;

public interface IFileBase
{
	public string ArchiveFolderBasePath { get; }
	public string DataTransferFolderBasePath { get; }
	public string ArchiveFolder { get; }
	public string ArchiveProcessedFolder { get; }
	public string ArchiveFailedFolder { get; } 
	public Task CleanUpArchiveFolder();
	public Task MoveToFolder(string sourceFile, string destinationFolder);
	public void CreateArchiveDirectory();
	public void DeleteFilesInDataTransferFolder();

}