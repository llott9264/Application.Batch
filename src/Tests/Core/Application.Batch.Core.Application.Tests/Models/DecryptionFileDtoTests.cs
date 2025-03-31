using Application.Batch.Core.Application.Models;

namespace Application.Batch.Core.Application.Tests.Models;

public class DecryptionFileDtoTests
{
	[Fact]
	public void DecryptionFileDto_CanBeCreated()
	{
		//Arrange
		const string archiveFolder = "MyArchiveFolder\\";
		const string dataTransferFolder = "MyDataTransferFolder\\";
		const string gpgFileName = "MyGpgFileName.txt.gpg";

		//Act
		DecryptionFileDto encryptionFileDto = new(archiveFolder, dataTransferFolder, gpgFileName);

		//Assert
		Assert.True(encryptionFileDto.ArchiveFileFullPath == $"{archiveFolder}MyGpgFileName.txt");
		Assert.True(encryptionFileDto.ArchiveGpgFileFullPath == $"{archiveFolder}{gpgFileName}");
		Assert.True(encryptionFileDto.DataTransferGpgFileFullPath == $"{dataTransferFolder}{gpgFileName}");
	}
}