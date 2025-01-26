namespace Application.Batch.Core.Application.Contracts.Application;

public interface IOutgoingFile
{
	internal string FileName { get; }
	internal string GpgFileName { get; }
	internal string ArchiveFileFullPath { get; }
	internal string ArchiveGpgFileFullPath { get; }
	internal string DataTransferGpgFullPath { get; }
	internal string GpgPublicKeyPath { get; }

	internal void EncryptFile();
	internal void MoveGpgFileToDataTransferFolder();
	internal void MoveArchiveFileToProcessedFolder();
	internal void MoveArchiveGpgFileToProcessFolder();

	internal void MoveArchiveFileToFailedFolder();
	internal void MoveArchiveGpgFileToFailedFolder();
	internal void CleanUpArchiveFolder();
	internal void CreateArchiveDirectory();
	internal void DeleteFilesInDataTransferFolder();
}