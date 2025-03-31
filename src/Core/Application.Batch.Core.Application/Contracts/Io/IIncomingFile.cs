namespace Application.Batch.Core.Application.Contracts.Io;

public interface IIncomingFile : IFileBase
{
	public Task MoveToGpgFileToArchiveFolder();
	public bool DoesArchiveGpgFileExist();
	public bool DoesArchiveFileExist();
	public Task DecryptFile();

	public Task MoveArchiveFileToProcessedFolder();
	public Task MoveArchiveFileToFailedFolder();
	public Task MoveArchiveGpgFileToProcessedFolder();
	public Task MoveArchiveGpgFileToFailedFolder();
}