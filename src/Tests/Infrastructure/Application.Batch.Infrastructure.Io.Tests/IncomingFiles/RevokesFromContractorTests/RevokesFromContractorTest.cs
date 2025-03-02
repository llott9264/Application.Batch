using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Models;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Infrastructure.Io.Tests.IncomingFiles.RevokesFromContractorTests;

public class RevokesFromContractorTest
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPublicKey.asc";
	private const string GpgPrivateKeyPassword = "password";

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyName"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyPassword"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
		return mock;
	}

	private static IMapper GetMapper()
	{
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		Mapper mapper = new(configuration);
		return mapper;
	}

	[Fact]
	public void CustomersFromContractor_PropertiesSetCorrectly_ReturnsTrue()
	{
		//Arrange
		string folderName = DateTime.Now.ToString("MMddyyyy");

		//Act
		RevokesFromContractor revokesFromContractor = new(GetMockMediator().Object, GetMapper());

		//Assert
		Assert.True(revokesFromContractor.BatchName == "Revokes From Contractor");
		Assert.True(revokesFromContractor.ArchiveFolderBasePath == $"{ArchiveFolderBasePath}");
		Assert.True(revokesFromContractor.DataTransferFolderBasePath == $"{DataTransferFolderBasePath}");
		Assert.True(revokesFromContractor.ArchiveFolder == $"{ArchiveFolderBasePath}{folderName}\\");
		Assert.True(revokesFromContractor.ArchiveProcessedFolder == $"{ArchiveFolderBasePath}{folderName}\\Processed\\");
		Assert.True(revokesFromContractor.ArchiveFailedFolder == $"{ArchiveFolderBasePath}{folderName}\\Failed\\");
		Assert.True(revokesFromContractor.GpgPrivateKeyName == $"{GpgPrivateKeyName}");
		Assert.True(revokesFromContractor.GpgPrivateKeyPassword == $"{GpgPrivateKeyPassword}");
	}


	[Fact]
	public void ReadFile_ValidFiles_ReturnsValidRevokeViewModel()
	{
		//Arrange
		RevokesFromContractor revokesFromContractor = new(GetMockMediator().Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");

		Utilities.IoOperations.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);
		File.Copy("IncomingFiles\\RevokesFromContractorTests\\File1.txt", $"{revokesFromContractor.ArchiveFolder}File1.txt", true);
		File.Copy("IncomingFiles\\RevokesFromContractorTests\\File2.txt", $"{revokesFromContractor.ArchiveFolder}File2.txt", true);


		//Act
		List<RevokeViewModel> revokeViewModels = revokesFromContractor.ReadFiles();

		//Assert
		Assert.True(revokeViewModels.Count == 4);
		Assert.True(revokeViewModels.Any(r => r.IsRevoked && r.SocialSecurityNumber == "123456789"));
		Assert.True(revokeViewModels.Any(r => !r.IsRevoked && r.SocialSecurityNumber == "234567890"));
		Assert.True(revokeViewModels.Any(r => r.IsRevoked && r.SocialSecurityNumber == "987654321"));
		Assert.True(revokeViewModels.Any(r => !r.IsRevoked && r.SocialSecurityNumber == "876543210"));
	}

	[Fact]
	public void ReadFile_InvalidFile_ThrowsException()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		Utilities.IoOperations.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);
		File.Copy("IncomingFiles\\RevokesFromContractorTests\\File3.txt", $"{revokesFromContractor.ArchiveFolder}File1.txt", true);

		//Act
		List<RevokeViewModel> revokeViewModels = revokesFromContractor.ReadFiles();

		//Assert
		Assert.True(revokeViewModels.Count == 0);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.LogType == LogType.Error
			&& request.Message.Contains("Error occurred reading file.")
		), CancellationToken.None), Times.Once);
	}
}