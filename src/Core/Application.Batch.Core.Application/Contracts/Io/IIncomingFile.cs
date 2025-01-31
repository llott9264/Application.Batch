namespace Application.Batch.Core.Application.Contracts.Io;

public interface IIncomingFile : IFileBase
{
	public void MoveToGpgFileToArchiveFolder();
	public bool DoesArchiveGpgFileExist();
	public bool DoesArchiveFileExist();
	public Task DecryptFile();
}