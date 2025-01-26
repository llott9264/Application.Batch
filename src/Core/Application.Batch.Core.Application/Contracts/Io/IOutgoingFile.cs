namespace Application.Batch.Core.Application.Contracts.Io;

public interface IOutgoingFile : IFileBase
{
	public string FileName { get; }
	public string GpgFileName { get; }
	public string ArchiveFileFullPath { get; }
	public string ArchiveGpgFileFullPath { get; }
	public string DataTransferGpgFullPath { get; }
	public string GpgPublicKeyPath { get; }
	public bool DoesArchiveGpgFileExist();
	public void EncryptFile();
	public void MoveGpgFileToDataTransferFolder();
	public void MoveArchiveFileToProcessedFolder();
	public void MoveArchiveGpgFileToProcessFolder();
	public void MoveArchiveFileToFailedFolder();
	public void MoveArchiveGpgFileToFailedFolder();
}