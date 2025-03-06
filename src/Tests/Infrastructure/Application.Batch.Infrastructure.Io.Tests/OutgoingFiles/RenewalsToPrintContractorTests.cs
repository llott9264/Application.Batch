using Application.Batch.Core.Application.Contracts.Pdf;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Tests.OutgoingFiles;

public class RenewalsToPrintContractorTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPublicKeyName = "MyPublicKey.asc";
	private const string PdfTemplatePath = "MyPdfTemplate.pdf";
	private const string DocumentsPerFile = "1";

	private Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RenewalsToPrintContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RenewalsToPrintContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RenewalsToPrintContractor:PublicKey"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPublicKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "WWorkflows:RenewalsToPrintContractor:PdfTemplatePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(PdfTemplatePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RenewalsToPrintContractor:DocumentsPerFile"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DocumentsPerFile));
		return mock;
	}

	private Mock<IRenewalsToPrintContractorPdf> GetMockPdf()
	{
		Mock<IRenewalsToPrintContractorPdf> mock = new();
		mock.Setup(m => m.CreatePdf(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Customer[]>()));
		return mock;
	}

	[Fact]
	public void RenewalsToPrintContractor_PropertiesSetCorrectly()
	{
		//Arrange
		string folderName = DateTime.Now.ToString("MMddyyyy");

		//Act
		RenewalsToPrintContractor renewalsToPrintContractor = new(GetMockMediator().Object, GetMockPdf().Object);

		//Assert
		Assert.True(renewalsToPrintContractor.BatchName == "Renewals To Print Contractor");
		Assert.True(renewalsToPrintContractor.ArchiveFolderBasePath == $"{ArchiveFolderBasePath}");
		Assert.True(renewalsToPrintContractor.DataTransferFolderBasePath == $"{DataTransferFolderBasePath}");
		Assert.True(renewalsToPrintContractor.ArchiveFolder == $"{ArchiveFolderBasePath}{folderName}\\");
		Assert.True(renewalsToPrintContractor.ArchiveProcessedFolder == $"{ArchiveFolderBasePath}{folderName}\\Processed\\");
		Assert.True(renewalsToPrintContractor.ArchiveFailedFolder == $"{ArchiveFolderBasePath}{folderName}\\Failed\\");
		Assert.True(renewalsToPrintContractor.GpgPublicKeyName == $"{GpgPublicKeyName}");
	}

	[Fact]
	public void RenewalsToPrintContractor_MethodsReturnCorrectly()
	{
		//Arrange
		string folderName = DateTime.Now.ToString("MMddyyyy");

		//Act
		RenewalsToPrintContractor renewalsToPrintContractor = new(GetMockMediator().Object, GetMockPdf().Object);

		//Assert
		Assert.True(renewalsToPrintContractor.GetArchiveFileFullPath("File1.txt") == $"{renewalsToPrintContractor.ArchiveFolder}File1.txt");
		Assert.True(renewalsToPrintContractor.GetArchiveGpgFileFullPath("File1.txt.gpg") == $"{renewalsToPrintContractor.ArchiveFolder}File1.txt.gpg");
		Assert.True(renewalsToPrintContractor.DataTransferGpgFullPath("File1.txt.gpg") == $"{renewalsToPrintContractor.DataTransferFolderBasePath}File1.txt.gpg");
	}

	[Fact]
	public void WriteFile_ValidCustomerList_ReturnsTrue()
	{
		//Arrange
		RenewalsToPrintContractor renewalsToPrintContractor = new(GetMockMediator().Object, GetMockPdf().Object);

		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Doe",
				SocialSecurityNumber = "123456789"
			},
			new()
			{
				FirstName = "Sam",
				LastName = "Jones",
				SocialSecurityNumber = "987654321"
			}
		];

		//Act
		bool isSuccessful = renewalsToPrintContractor.WriteFiles(customers);

		//Assert
		Assert.True(isSuccessful);
		Assert.True(renewalsToPrintContractor.Files.Count == 2);
		Assert.Contains("Renewals_1.pdf", renewalsToPrintContractor.Files[0].ArchiveFileFullPath);
		Assert.Contains("Renewals_2.pdf", renewalsToPrintContractor.Files[1].ArchiveFileFullPath);
	}

	[Fact]
	public void WriteFile_ValidCustomerList_ThrowsException()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RenewalsToPrintContractor:DocumentsPerFile"), It.IsAny<CancellationToken>())).Returns(Task.FromResult("0"));
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);

		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Doe",
				SocialSecurityNumber = "123456789"
			},
			new()
			{
				FirstName = "Sam",
				LastName = "Jones",
				SocialSecurityNumber = "987654321"
			}
		];

		//Act
		bool isSuccessful = renewalsToPrintContractor.WriteFiles(customers);

		//Assert
		Assert.False(isSuccessful);
		Assert.True(renewalsToPrintContractor.Files.Count == 0);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.LogType == LogType.Error
			&& request.Message.Contains("Error occurred writing file.")
		), CancellationToken.None), Times.Once);
	}
}
