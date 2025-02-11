using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Infrastructure.Io.IncomingFiles;

namespace Application.Batch.Infrastructure.Io.Tests
{
	public class IncomingFileTests
	{
		[Fact]
		public void CustomersFromContractor_PropertiesSetCorrectly_ReturnsTrue()
		{
			//Arrange
			string folderName = DateTime.Now.ToString("MMddyyyy");
			const string folderBase = "MyFolderPath\\";

			//Act
			CustomersFromContractor customersFromContractor = new(Helper.MockMediator().Object, Helper.MockMapper().Object);

			//Assert
			Assert.True(customersFromContractor.BatchName == "Customers From Print Contractor");
			Assert.True(customersFromContractor.ArchiveFolderBasePath == $"{folderBase}");
			Assert.True(customersFromContractor.DataTransferFolderBasePath == $"{folderBase}");
			Assert.True(customersFromContractor.ArchiveFolder == $"{folderBase}{folderName}\\");
			Assert.True(customersFromContractor.ArchiveProcessedFolder == $"{folderBase}{folderName}\\Processed\\");
			Assert.True(customersFromContractor.ArchiveFailedFolder == $"{folderBase}{folderName}\\Failed\\");
			Assert.True(customersFromContractor.FileName == "CustomerList.txt");
			Assert.True(customersFromContractor.GpgFileName == "CustomerList.txt.gpg");
			Assert.True(customersFromContractor.GpgPrivateKeyName == $"{folderBase}");
			Assert.True(customersFromContractor.GpgPrivateKeyPassword == $"{folderBase}");
			Assert.True(customersFromContractor.DataTransferGpgFullPath == $"{folderBase}\\CustomerList.txt.gpg");
			Assert.True(customersFromContractor.ArchiveFileFullPath == $"{folderBase}{folderName}\\CustomerList.txt");
			Assert.True(customersFromContractor.ArchiveGpgFileFullPath == $"{folderBase}{folderName}\\CustomerList.txt.gpg");
		}

		[Fact]
		public async void ReadFile_ValidCustomerViewModel_ReturnsTrue()
		{
			//Arrange
			CustomersFromContractor customersFromContractor = new(Helper.MockMediator().Object, Helper.MockMapper().Object);

			//Act
			List<CustomerViewModel> customerViewModels = await customersFromContractor.ReadFile();

			//Assert


		}
	}
}