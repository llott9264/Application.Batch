using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Features.Utilities.Gpg.Commands;
using MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;

public abstract class OutgoingFile(
	IMediator mediator,
	string archiveFolderBasePath,
	string dataTransferFolderBasePath,
	string fileName,
	string gpgFileName,
	string gpgPublicKeyPath) : FileBase(mediator, archiveFolderBasePath, dataTransferFolderBasePath), IOutgoingFile
{
	public string FileName { get; } = fileName;
	public string GpgFileName { get; } = gpgFileName;
	public string GpgPublicKeyPath { get; } = gpgPublicKeyPath;
	public string ArchiveFileFullPath => $@"{ArchiveFolder}\{FileName}";
	public string ArchiveGpgFileFullPath => $@"{ArchiveFolder}\{GpgFileName}";
	public string DataTransferGpgFullPath => $@"{DataTransferFolderBasePath}\{GpgFileName}";

	public async Task EncryptFile()
	{
		await Mediator.Send(new EncryptFileCommand(ArchiveFileFullPath, ArchiveGpgFileFullPath, GpgPublicKeyPath));
	}

	public bool DoesArchiveGpgFileExist()
	{
		return File.Exists(ArchiveGpgFileFullPath);
	}
	public void MoveGpgFileToDataTransferFolder()
	{
		File.Copy(ArchiveGpgFileFullPath, DataTransferGpgFullPath);
	}

	public async Task MoveArchiveFileToProcessedFolder()
	{
		await MoveToFolder(ArchiveFileFullPath, ArchiveProcessedFolder);
		await MoveToFolder(ArchiveGpgFileFullPath, ArchiveProcessedFolder);
	}

	public async Task MoveArchiveGpgFileToProcessFolder()
	{
		await MoveToFolder(ArchiveGpgFileFullPath, ArchiveProcessedFolder);
	}

	public async Task MoveArchiveFileToFailedFolder()
	{
		await MoveToFolder(ArchiveFileFullPath, ArchiveFailedFolder);
	}
	public async Task MoveArchiveGpgFileToFailedFolder()
	{
		await MoveToFolder(ArchiveGpgFileFullPath, ArchiveFailedFolder);
	}
}