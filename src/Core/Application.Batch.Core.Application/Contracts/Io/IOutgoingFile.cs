﻿namespace Application.Batch.Core.Application.Contracts.Io;

public interface IOutgoingFile : IFileBase
{
	public string FileName { get; }
	public string GpgFileName { get; }
	public string ArchiveFileFullPath { get; }
	public string ArchiveGpgFileFullPath { get; }
	public string DataTransferGpgFullPath { get; }
	public string GpgPublicKeyName { get; }
	public bool DoesArchiveFileExist();
	public bool DoesArchiveGpgFileExist();
	public Task EncryptFile();
	public Task CopyGpgFileToDataTransferFolder();
	public Task MoveArchiveFileToProcessedFolder();
	public Task MoveArchiveGpgFileToProcessedFolder();
	public Task MoveArchiveFileToFailedFolder();
	public Task MoveArchiveGpgFileToFailedFolder();
}