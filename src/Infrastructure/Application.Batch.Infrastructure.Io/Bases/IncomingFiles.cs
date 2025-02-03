using System.Linq.Expressions;
using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Utilities.Gpg.Commands;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Application.Models;
using MediatR;

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
			Path.GetFileNameWithoutExtension(fileName),
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
		foreach (DecryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveFileFullPath, ArchiveProcessedFolder);
		}
	}

	public void MoveArchiveGpgFilesToProcessFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveProcessedFolder);
		}
	}

	public void MoveArchiveFilesToFailedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveFileFullPath, ArchiveFailedFolder);
		}
	}

	public void MoveArchiveGpgFilesToFailedFolder()
	{
		foreach (DecryptionFileDto file in Files)
		{
			MoveToFolder(file.ArchiveGpgFileFullPath, ArchiveFailedFolder);
		}
	}

	public async Task<bool> MoveGpgFilesToArchiveFolder()
	{
		bool isSuccessful = false;

		try
		{
			await Mediator.Send(new CreateLogCommand("Begin copying gpg files to archive transfer.", LogType.Information));
			Files.ForEach(f => File.Move(f.DataTransferGpgFileFullPath, f.ArchiveGpgFileFullPath, true));
			await Mediator.Send(new CreateLogCommand("Successfully copied gpg files to archive folder.", LogType.Information));
			isSuccessful = true;
		}
		catch (Exception e)
		{
			await Mediator.Send(new CreateLogCommand($"Failure to copy gpg files to archive folder. Error Message: {e.Message}", LogType.Error));
		}

		await Mediator.Send(new CreateLogCommand("End copying gpg files to archive folder.", LogType.Error));
		return isSuccessful;
	}

	public void GetGpgFilesInDataTransferFolder()
	{
		List<FileInfo> files = new DirectoryInfo(DataTransferFolderBasePath).GetFiles()
			.Where(f => f.Extension == ".gpg")
			.OrderBy(f => f.CreationTime)
			.ToList();

		files.ForEach(f => Files.Add(new DecryptionFileDto(ArchiveFolder, DataTransferFolderBasePath, Path.GetFileNameWithoutExtension(f.Name), f.Name)));
	}
}