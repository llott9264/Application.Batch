using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Tests.IncomingFiles.CustomersFromContractorTests;

public class CustomersFromContractorTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\CustomersFromContractor\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\CustomersFromContractor\\";
	private const string GpgPrivateKeyName = "MyPublicKey.asc";
	private const string GpgPrivateKeyPassword = "password";

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m =>
				m.Send(
					It.Is<GetConfigurationByKeyQuery>(request =>
						request.Key == "Workflows:CustomersFromContractor:ArchivePath"), It.IsAny<CancellationToken>()))
			.Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m =>
			m.Send(
				It.Is<GetConfigurationByKeyQuery>(request =>
					request.Key == "Workflows:CustomersFromContractor:DataTransferPath"),
				It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m =>
				m.Send(
					It.Is<GetConfigurationByKeyQuery>(request =>
						request.Key == "Workflows:CustomersFromContractor:PrivateKeyName"),
					It.IsAny<CancellationToken>()))
			.Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m =>
			m.Send(
				It.Is<GetConfigurationByKeyQuery>(request =>
					request.Key == "Workflows:CustomersFromContractor:PrivateKeyPassword"),
				It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
		return mock;
	}

	private static IMapper GetMapper()
	{
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		Mapper mapper = new(configuration);
		return mapper;
	}

	[Fact]
	public void CustomersFromContractor_PropertiesSetCorrectly()
	{
		//Arrange
		string folderName = DateTime.Now.ToString("MMddyyyy");

		//Act
		ICustomersFromContractor customersFromContractor =
			new CustomersFromContractor(GetMockMediator().Object, GetMapper());

		//Assert
		Assert.True(customersFromContractor.BatchName == "Customers From Print Contractor");
		Assert.True(customersFromContractor.ArchiveFolderBasePath == $"{ArchiveFolderBasePath}");
		Assert.True(customersFromContractor.DataTransferFolderBasePath == $"{DataTransferFolderBasePath}");
		Assert.True(customersFromContractor.ArchiveFolder == $"{ArchiveFolderBasePath}{folderName}\\");
		Assert.True(customersFromContractor.ArchiveProcessedFolder ==
					$"{ArchiveFolderBasePath}{folderName}\\Processed\\");
		Assert.True(customersFromContractor.ArchiveFailedFolder == $"{ArchiveFolderBasePath}{folderName}\\Failed\\");
		Assert.True(customersFromContractor.FileName == "CustomerList.txt");
		Assert.True(customersFromContractor.GpgFileName == "CustomerList.txt.gpg");
		Assert.True(customersFromContractor.GpgPrivateKeyName == $"{GpgPrivateKeyName}");
		Assert.True(customersFromContractor.GpgPrivateKeyPassword == $"{GpgPrivateKeyPassword}");
		Assert.True(customersFromContractor.DataTransferGpgFullPath ==
					$"{DataTransferFolderBasePath}CustomerList.txt.gpg");
		Assert.True(customersFromContractor.ArchiveFileFullPath ==
					$"{ArchiveFolderBasePath}{folderName}\\CustomerList.txt");
		Assert.True(customersFromContractor.ArchiveGpgFileFullPath ==
					$"{ArchiveFolderBasePath}{folderName}\\CustomerList.txt.gpg");
	}

	[Fact]
	public async Task ReadFile_ValidFile_ReturnsValidCustomerViewModel()
	{
		//Arrange
		ICustomersFromContractor customersFromContractor =
			new CustomersFromContractor(GetMockMediator().Object, GetMapper());

		if (!Directory.Exists(customersFromContractor.ArchiveFolder))
		{
			Directory.CreateDirectory(customersFromContractor.ArchiveFolder);
		}

		File.Copy("IncomingFiles\\CustomersFromContractorTests\\CustomerList.txt",
			customersFromContractor.ArchiveFileFullPath, true);

		//Act
		List<CustomerViewModel> customerViewModels = await customersFromContractor.ReadFile();

		//Assert
		Assert.True(customerViewModels.Count == 2);
		Assert.True(customerViewModels[0].FirstName == "John");
		Assert.True(customerViewModels[0].LastName == "Doe");
		Assert.True(customerViewModels[0].SocialSecurityNumber == "123456789");
		Assert.True(customerViewModels[1].FirstName == "Sam");
		Assert.True(customerViewModels[1].LastName == "Smith");
		Assert.True(customerViewModels[1].SocialSecurityNumber == "987654321");
	}

	[Fact]
	public async Task ReadFile_ValidFile_ThrowsException()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		ICustomersFromContractor customersFromContractor =
			new CustomersFromContractor(mock.Object, GetMapper());

		if (File.Exists(customersFromContractor.ArchiveFileFullPath))
		{
			File.Delete(customersFromContractor.ArchiveFileFullPath);
		}

		//Act
		await customersFromContractor.ReadFile();

		//Assert
		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.LogType == LogType.Error
			&& request.Message.Contains("Error occurred reading file.")
		), CancellationToken.None), Times.Once);
	}
}
