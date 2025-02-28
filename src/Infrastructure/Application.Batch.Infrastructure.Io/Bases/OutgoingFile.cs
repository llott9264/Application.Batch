using Application.Batch.Core.Application.Contracts.Io;
using MediatR;
using Utilities.Gpg.MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;

public abstract class OutgoingFile(
	IMediator mediator,
	string archiveFolderBasePath,
	string dataTransferFolderBasePath,
	string fileName,
	string gpgFileName,
	string gpgPublicKeyName) : FileBase(mediator, archiveFolderBasePath, dataTransferFolderBasePath), IOutgoingFile
{
	public string FileName { get; } = fileName;
	public string GpgFileName { get; } = gpgFileName;
	public string GpgPublicKeyName { get; } = gpgPublicKeyName;
	public string ArchiveFileFullPath => $@"{ArchiveFolder}{FileName}";
	public string ArchiveGpgFileFullPath => $@"{ArchiveFolder}{GpgFileName}";
	public string DataTransferGpgFullPath => $@"{DataTransferFolderBasePath}{GpgFileName}";

	public async Task EncryptFile()
	{
		await Mediator.Send(new EncryptFileCommand(ArchiveFileFullPath, ArchiveGpgFileFullPath, GpgPublicKeyName));
	}

	public bool DoesArchiveFileExist()
	{
		return File.Exists(ArchiveFileFullPath);
	}

	public bool DoesArchiveGpgFileExist()
	{
		return File.Exists(ArchiveGpgFileFullPath);
	}
	public void MoveGpgFileToDataTransferFolder()
	{
		File.Copy(ArchiveGpgFileFullPath, DataTransferGpgFullPath);
	}

	public void MoveArchiveFileToProcessedFolder()
	{
		MoveToFolder(ArchiveFileFullPath, ArchiveProcessedFolder);
	}

	public void MoveArchiveGpgFileToProcessFolder()
	{
		MoveToFolder(ArchiveGpgFileFullPath, ArchiveProcessedFolder);
	}

	public void MoveArchiveFileToFailedFolder()
	{
		MoveToFolder(ArchiveFileFullPath, ArchiveFailedFolder);
	}
	public void MoveArchiveGpgFileToFailedFolder()
	{
		MoveToFolder(ArchiveGpgFileFullPath, ArchiveFailedFolder);
	}
}