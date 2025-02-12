namespace Application.Batch.Core.Application.Models;

public class EncryptionFileDto(
	string archiveFolder,
	string dataTransferFolderBasePath,
	string fileName,
	string gpgFileName)
{
	public string ArchiveFileFullPath => $@"{archiveFolder}{fileName}";
	public string ArchiveGpgFileFullPath => $@"{archiveFolder}{gpgFileName}.gpg";
	public string DataTransferGpgFileFullPath => $@"{dataTransferFolderBasePath}{gpgFileName}.gpg";
}
