using Application.Batch.Core.Application.Models;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface IOutgoingFiles : IFileBase
{
	List<EncryptionFileDto> Files { get; }
	string GpgPublicKeyName { get; }
	string GetArchiveFileFullPath(string fileName);
	string GetArchiveGpgFileFullPath(string fileName);
	string DataTransferGpgFullPath(string fileName);
	Task<bool> EncryptFiles();
	void AddFileToEncrypt(string fileName);
	bool DoArchiveGpgFilesExist();
	Task<bool> MoveGpgFilesToDataTransferFolder();
	void MoveArchiveFilesToProcessedFolder();
	void MoveArchiveGpgFilesToProcessFolder();
	void MoveArchiveFilesToFailedFolder();
	void MoveArchiveGpgFilesToFailedFolder();
}