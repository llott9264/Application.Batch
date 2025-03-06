using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;

namespace Application.Batch.Infrastructure.Io.Tests.OutgoingFiles
{
	public class CustomersToPrintContractorTests
	{
		private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\CustomersToPrintContractor\\";
		private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\CustomersToPrintContractor\\";
		private const string GpgPublicKeyName = "MyPublicKey.asc";

		private Mock<IMediator> GetMockMediator()
		{
			Mock<IMediator> mock = new();
			mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersToPrintContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
			mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersToPrintContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
			mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersToPrintContractor:PublicKey"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPublicKeyName));
			return mock;
		}

		[Fact]
		public void CustomerToPrintContractor_PropertiesSetCorrectly()
		{
			//Arrange
			string folderName = DateTime.Now.ToString("MMddyyyy");

			//Act
			CustomersToPrintContractor customersFromContractor = new(GetMockMediator().Object);

			//Assert
			Assert.True(customersFromContractor.BatchName == "Customer To Print Contractor");
			Assert.True(customersFromContractor.ArchiveFolderBasePath == $"{ArchiveFolderBasePath}");
			Assert.True(customersFromContractor.DataTransferFolderBasePath == $"{DataTransferFolderBasePath}");
			Assert.True(customersFromContractor.ArchiveFolder == $"{ArchiveFolderBasePath}{folderName}\\");
			Assert.True(customersFromContractor.ArchiveProcessedFolder == $"{ArchiveFolderBasePath}{folderName}\\Processed\\");
			Assert.True(customersFromContractor.ArchiveFailedFolder == $"{ArchiveFolderBasePath}{folderName}\\Failed\\");
			Assert.True(customersFromContractor.FileName == "CustomerList.txt");
			Assert.True(customersFromContractor.GpgFileName == "CustomerList.txt.gpg");
			Assert.True(customersFromContractor.GpgPublicKeyName == $"{GpgPublicKeyName}");
			Assert.True(customersFromContractor.DataTransferGpgFullPath == $"{DataTransferFolderBasePath}CustomerList.txt.gpg");
			Assert.True(customersFromContractor.ArchiveFileFullPath == $"{ArchiveFolderBasePath}{folderName}\\CustomerList.txt");
			Assert.True(customersFromContractor.ArchiveGpgFileFullPath == $"{ArchiveFolderBasePath}{folderName}\\CustomerList.txt.gpg");
		}

		[Fact]
		public void WriteFile_ValidCustomer_ReturnsTrue()
		{
			//Arrange
			CustomersToPrintContractor customerToPrintContractor = new(GetMockMediator().Object);
			Directory.CreateDirectory(Path.GetDirectoryName(customerToPrintContractor.ArchiveFileFullPath));

			List<Customer> customers = new()
			{
				new Customer()
				{
					FirstName = "John",
					LastName = "Doe",
					SocialSecurityNumber = "123456789"
				}
			};

			//Act
			bool isSuccessful = customerToPrintContractor.WriteFile(customers);

			//Assert
			Assert.True(isSuccessful);
			Assert.True(File.Exists(customerToPrintContractor.ArchiveFileFullPath));

			string line1 = File.ReadLines(customerToPrintContractor.ArchiveFileFullPath).First();
			Assert.True(line1 == "Doe,John123456789");
		}
	}
}