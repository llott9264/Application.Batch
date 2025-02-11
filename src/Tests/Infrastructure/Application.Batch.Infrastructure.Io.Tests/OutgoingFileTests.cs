using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using System.Collections.Generic;

namespace Application.Batch.Infrastructure.Io.Tests
{
	public class OutgoingFileTests
	{
		[Fact]
		public void CustomerToPrintContractor_PropertiesSetCorrectly_ReturnsTrue()
		{
			//Arrange
			string folderName = DateTime.Now.ToString("MMddyyyy");
			const string folderBase = "MyFolderPath\\";

			//Act
			CustomerToPrintContractor customersFromContractor = new(Helper.MockMediator().Object);

			//Assert
			Assert.True(customersFromContractor.BatchName == "Customer To Print Contractor");
			Assert.True(customersFromContractor.ArchiveFolderBasePath == $"{folderBase}");
			Assert.True(customersFromContractor.DataTransferFolderBasePath == $"{folderBase}");
			Assert.True(customersFromContractor.ArchiveFolder == $"{folderBase}{folderName}\\");
			Assert.True(customersFromContractor.ArchiveProcessedFolder == $"{folderBase}{folderName}\\Processed\\");
			Assert.True(customersFromContractor.ArchiveFailedFolder == $"{folderBase}{folderName}\\Failed\\");
			Assert.True(customersFromContractor.FileName == "CustomerList.txt");
			Assert.True(customersFromContractor.GpgFileName == "CustomerList.txt.gpg");
			Assert.True(customersFromContractor.GpgPublicKeyName == $"{folderBase}");
			Assert.True(customersFromContractor.DataTransferGpgFullPath == $"{folderBase}CustomerList.txt.gpg");
			Assert.True(customersFromContractor.ArchiveFileFullPath == $"{folderBase}{folderName}\\CustomerList.txt");
			Assert.True(customersFromContractor.ArchiveGpgFileFullPath == $"{folderBase}{folderName}\\CustomerList.txt.gpg");
		}

		[Fact]
		public void WriteFile_ValidCustomer_ReturnsTrue()
		{
			//Arrange
			CustomerToPrintContractor customerToPrintContractor = new(Helper.MockMediator().Object);
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