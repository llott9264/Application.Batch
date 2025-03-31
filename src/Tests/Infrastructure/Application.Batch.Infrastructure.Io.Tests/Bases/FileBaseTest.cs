using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.IoOperations.MediatR.Directory.CleanUpDirectory;
using Utilities.IoOperations.MediatR.Directory.CreateDirectory;
using Utilities.IoOperations.MediatR.Directory.DeleteFiles;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;
using AutoMapper;

namespace Application.Batch.Infrastructure.Io.Tests.Bases;

public class FileBaseTest
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPublicKey.asc";
	private const string GpgPrivateKeyPassword = "password";
	private readonly string _folderName = DateTime.Now.ToString("MMddyyyy");

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyName"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyPassword"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "FileRetentionPeriodInMonths"), It.IsAny<CancellationToken>())).Returns(Task.FromResult("-13"));
		return mock;
	}

	private static IMapper GetMapper()
	{
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		Mapper mapper = new(configuration);
		return mapper;
	}

	[Fact]
	public void CleanUpArchiveFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		_ = revokesFromContractor.CleanUpArchiveFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "FileRetentionPeriodInMonths"), CancellationToken.None), Times.Exactly(1));

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message.Contains("Begin cleaning up of the Archive folder")
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Exactly(1));

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message.Contains("End cleaning up of the Archive folder")
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Exactly(1));

		mock.Verify(g => g.Send(It.Is<CleanUpDirectoryCommand>(request =>
			request.Directory.FullName == new DirectoryInfo(ArchiveFolderBasePath).FullName
			&& request.RetentionLengthInMonths == -13
			&& request.IsBaseFolder == true), CancellationToken.None), Times.Exactly(1));
	}
	[Fact]
	public void CreateArchiveDirectory_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		_ = revokesFromContractor.CreateArchiveDirectory();

		//Assert
		mock.Verify(g => g.Send(It.Is<CreateDirectoryCommand>(request =>
			request.Folder == $"{ArchiveFolderBasePath}{_folderName}\\"), CancellationToken.None), Times.Exactly(1));
	}
	[Fact]
	public void DeleteFilesInDataTransferFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		_ = revokesFromContractor.DeleteFilesInDataTransferFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<DeleteFilesCommand>(request =>
			request.Directory.FullName == new DirectoryInfo(DataTransferFolderBasePath).FullName), CancellationToken.None), Times.Exactly(1));
	}
}