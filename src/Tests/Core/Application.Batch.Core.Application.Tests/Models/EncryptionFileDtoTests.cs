using Application.Batch.Core.Application.Models;

namespace Application.Batch.Core.Application.Tests.Models
{
	public class EncryptionFileDtoTests
	{
		[Fact]
		public void EncryptionFileDto_CanBeCreated()
		{
			//Arrange
			const string archiveFolder = "MyArchiveFolder\\";
			const string dataTransferFolder = "MyDataTransferFolder\\";
			const string fileName = "MyFileName.txt";


			//Act
			EncryptionFileDto encryptionFileDto = new(archiveFolder, dataTransferFolder, fileName);

			//Assert
			Assert.True(encryptionFileDto.ArchiveFileFullPath == $"{archiveFolder}{fileName}");
			Assert.True(encryptionFileDto.ArchiveGpgFileFullPath == $"{archiveFolder}{fileName}.gpg");
			Assert.True(encryptionFileDto.DataTransferGpgFileFullPath == $"{dataTransferFolder}{fileName}.gpg");
		}
	}
}