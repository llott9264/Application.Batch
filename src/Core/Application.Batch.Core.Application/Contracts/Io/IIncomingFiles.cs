using Application.Batch.Core.Application.Models;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface IIncomingFiles : IFileBase
{
	public List<DecryptionFileDto> Files { get;}
	public string GpgPrivateKeyName { get; }
	public string GpgPrivateKeyPassword { get; }
	public string GetArchiveFileFullPath(string fileName);
	public string GetArchiveGpgFileFullPath(string fileName);
	public string DataTransferGpgFullPath(string fileName);
	public Task<bool> DecryptFiles();
	public void AddFileToDecrypt(string fileName);
	public bool DoArchiveGpgFilesExist();
	public bool DoArchiveFilesExist();
	public void MoveArchiveFilesToProcessedFolder();
	public void MoveArchiveGpgFilesToProcessFolder();
	public void MoveArchiveFilesToFailedFolder();
	public void MoveArchiveGpgFilesToFailedFolder();
	public Task<bool> MoveGpgFilesToArchiveFolder();
	public void GetGpgFilesInDataTransferFolder();
}