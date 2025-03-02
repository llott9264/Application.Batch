using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Models;
using MediatR;
using Utilities.Gpg.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;

public abstract class OutgoingFiles(IMediator mediator,
	string archiveFolderBasePath,
	string dataTransferFolderBasePath,
	string gpgPublicKeyName) : FileBase(mediator, archiveFolderBasePath, dataTransferFolderBasePath), IOutgoingFiles
{
	public List<EncryptionFileDto> Files{ get; } = new();
	public string GpgPublicKeyName { get; } = gpgPublicKeyName;

	public string GetArchiveFileFullPath(string fileName)
	{
		return Path.Combine(ArchiveFolder, fileName);
	}

	public string GetArchiveGpgFileFullPath(string fileName)
	{
		return Path.Combine(ArchiveFolder, fileName);
	}

	public string DataTransferGpgFullPath(string fileName)
	{
		return Path.Combine(DataTransferFolderBasePath, fileName);
	}

	public async Task<bool> EncryptFiles()
	{
		bool isSuccessful = false;
		try
		{
			foreach (EncryptionFileDto file in Files)
			{

				await Mediator.Send(new EncryptFileCommand(file.ArchiveFileFullPath,
					file.ArchiveGpgFileFullPath,
					GpgPublicKeyName));
			}

			isSuccessful = true;
		}
		catch (Exception e)
		{
			await Mediator.Send(new CreateLogCommand($"Failed to Encrypt all files.  Error Message {e.Message}", LogType.Error));
		}

		return isSuccessful;
	}

	public void AddFileToEncrypt(string fileName)
	{
		Files.Add(new EncryptionFileDto(ArchiveFolder,
			DataTransferFolderBasePath,
			fileName));
	}

	public bool DoArchiveGpgFilesExist()
	{
		return Files.Aggregate(true, (current, file) => current && File.Exists(file.ArchiveGpgFileFullPath));
	}

	public bool DoArchiveFilesExist()
	{
		return Files.Aggregate(true, (current, file) => current && File.Exists(file.ArchiveFileFullPath));
	}

	public void MoveArchiveFilesToProcessedFolder()
	{
		foreach (EncryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveFileFullPath, ArchiveProcessedFolder);
		}
	}

	public void MoveArchiveGpgFilesToProcessFolder()
	{
		foreach (EncryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveProcessedFolder);
		}
	}

	public void MoveArchiveFilesToFailedFolder()
	{
		foreach (EncryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveFileFullPath, ArchiveFailedFolder);
		}
	}

	public void MoveArchiveGpgFilesToFailedFolder()
	{
		foreach (EncryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveFailedFolder);
		}
	}

	public async Task<bool> CopyGpgFilesToDataTransferFolder()
	{
		bool isSuccessful = false;

		try
		{
			await Mediator.Send(new CreateLogCommand("Begin copying archive files to data transfer.", LogType.Information));
			Files.ForEach(f => File.Copy(f.ArchiveGpgFileFullPath, f.DataTransferGpgFileFullPath, true));
			await Mediator.Send(new CreateLogCommand("Successfully copied gpg files to data transfer folder.", LogType.Information));
			isSuccessful = true;
		}
		catch (Exception e)
		{
			await Mediator.Send(new CreateLogCommand($"Failure to copy archive files to data transfer. Error Message: {e.Message}", LogType.Information));
		}

		await Mediator.Send(new CreateLogCommand("End copying archive files to data transfer.", LogType.Error));
		return isSuccessful;
	}
}
