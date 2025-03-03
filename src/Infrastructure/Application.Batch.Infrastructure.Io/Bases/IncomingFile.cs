using Application.Batch.Core.Application.Contracts.Io;
using MediatR;
using Utilities.Gpg.MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;

public abstract class IncomingFile(
	IMediator mediator,
	string archiveFolderBasePath,
	string dataTransferFolderBasePath,
	string gpgPrivateKeyName,
	string gpgPrivateKeyPassword,
	string fileName,
	string gpgFileName) : FileBase(mediator, archiveFolderBasePath, dataTransferFolderBasePath), IIncomingFile
{
	public string FileName { get; } = fileName;
	public string GpgFileName { get; } = gpgFileName;
	public string GpgPrivateKeyName { get; } = gpgPrivateKeyName;
	public string GpgPrivateKeyPassword { get; } = gpgPrivateKeyPassword;
	public string DataTransferGpgFullPath => $@"{DataTransferFolderBasePath}\{GpgFileName}";
	public string ArchiveFileFullPath => $@"{ArchiveFolder}{FileName}";
	public string ArchiveGpgFileFullPath => $@"{ArchiveFolder}{GpgFileName}";
	public bool DoesArchiveFileExist()
	{
		throw new NotImplementedException();
	}
	public bool DoesArchiveGpgFileExist()
	{
		throw new NotImplementedException();
	}
	public async Task DecryptFile()
	{
		await Mediator.Send(new DecryptFileCommand(ArchiveGpgFileFullPath, ArchiveFileFullPath, GpgPrivateKeyName, GpgPrivateKeyPassword));
	}

	public async Task MoveToGpgFileToArchiveFolder()
	{
		await MoveToFolder(DataTransferGpgFullPath, ArchiveFolder);
	}
}