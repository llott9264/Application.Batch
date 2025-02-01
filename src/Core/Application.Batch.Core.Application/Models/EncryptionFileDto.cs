namespace Application.Batch.Core.Application.Models;

public class EncryptionFileDto(
	string archiveFolder,
	string dataTransferFolderBasePath,
	string fileName,
	string gpgFileName)
{
	public string ArchiveFileFullPath => $@"{archiveFolder}{fileName}";
	public string ArchiveGpgFileFullPath => $@"{archiveFolder}{gpgFileName}";
	public string DataTransferGpgFileFullPath => $@"{dataTransferFolderBasePath}{gpgFileName}";
}