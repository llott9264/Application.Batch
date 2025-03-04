using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Models;
using MediatR;
using Utilities.Gpg.MediatR;
using Utilities.IoOperations.MediatR.File.MoveFile;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Bases;

public abstract class IncomingFiles(
	IMediator mediator,
	string archiveFolderBasePath,
	string dataTransferFolderBasePath,
	string gpgPrivateKeyName,
	string gpgPrivateKeyPassword) : FileBase(mediator, archiveFolderBasePath, dataTransferFolderBasePath), IIncomingFiles
{
	public List<DecryptionFileDto> Files { get; } = new();

	public string GpgPrivateKeyName { get; } = gpgPrivateKeyName;
	public string GpgPrivateKeyPassword { get; } = gpgPrivateKeyPassword;

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

	public async Task<bool> DecryptFiles()
	{
		bool isSuccessful = false;
		try
		{
			foreach (DecryptionFileDto file in Files)
			{

				await Mediator.Send(new DecryptFileCommand(file.ArchiveGpgFileFullPath,
					file.ArchiveFileFullPath,
					GpgPrivateKeyName, GpgPrivateKeyPassword));
			}

			isSuccessful = true;
		}
		catch (Exception e)
		{
			await Mediator.Send(new CreateLogCommand($"Failed to Decrypt all files.  Error Message {e.Message}", LogType.Error));
		}

		return isSuccessful;
	}

	public void AddFileToDecrypt(string fileName)
	{
		Files.Add(new DecryptionFileDto(ArchiveFolder,
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

	public async Task MoveArchiveFilesToProcessedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			await MoveToFolder(file.ArchiveFileFullPath, ArchiveProcessedFolder);
		}
	}

	public async Task MoveArchiveGpgFilesToProcessedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			await MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveProcessedFolder);
		}
	}

	public async Task MoveArchiveFilesToFailedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			await MoveToFolder(file.ArchiveFileFullPath, ArchiveFailedFolder);
		}
	}

	public async Task MoveArchiveGpgFilesToFailedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			await MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveFailedFolder);
		}
	}

	public async Task<bool> MoveGpgFilesToArchiveFolder()
	{
		bool isSuccessful = false;

		try
		{
			await Mediator.Send(new CreateLogCommand("Begin copying gpg files to archive transfer.", LogType.Information));

			foreach (DecryptionFileDto file in Files)
			{
				await Mediator.Send(new MoveFileCommand(file.DataTransferGpgFileFullPath, file.ArchiveGpgFileFullPath));
			}

			await Mediator.Send(new CreateLogCommand("Successfully copied gpg files to archive folder.", LogType.Information));
			isSuccessful = true;
		}
		catch (Exception e)
		{
			await Mediator.Send(new CreateLogCommand($"Failure to copy gpg files to archive folder. Error Message: {e.Message}", LogType.Error));
		}

		await Mediator.Send(new CreateLogCommand("End copying gpg files to archive folder.", LogType.Information));
		return isSuccessful;
	}

	public void GetGpgFilesInDataTransferFolder()
	{
		List<FileInfo> files = new DirectoryInfo(DataTransferFolderBasePath).GetFiles()
			.Where(f => f.Extension == ".gpg")
			.OrderBy(f => f.CreationTime)
			.ToList();

		files.ForEach(f => Files.Add(new DecryptionFileDto(ArchiveFolder, DataTransferFolderBasePath, f.Name)));
	}
}