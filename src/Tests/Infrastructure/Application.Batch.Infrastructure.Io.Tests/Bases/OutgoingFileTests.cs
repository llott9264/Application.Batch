using Application.Batch.Core.Application.Models;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Gpg.MediatR;
using Utilities.IoOperations.MediatR.File.CopyFile;
using Utilities.IoOperations.MediatR.File.MoveFile;

namespace Application.Batch.Infrastructure.Io.Tests.Bases;


public class OutgoingFileTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
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
	public void DoesArchiveFileExist_TestFileExistsAndDoesNotExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);
		
		if (!System.IO.Directory.Exists(customerToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(customerToPrintContractor.ArchiveFolder);

		if (System.IO.File.Exists(customerToPrintContractor.ArchiveFileFullPath))
		{
			File.Delete(customerToPrintContractor.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(customerToPrintContractor.ArchiveFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doesExist = customerToPrintContractor.DoesArchiveFileExist();

		if (System.IO.File.Exists(customerToPrintContractor.ArchiveFileFullPath))
		{
			File.Delete(customerToPrintContractor.ArchiveFileFullPath);
		}

		bool doesNotExist = customerToPrintContractor.DoesArchiveFileExist();


		//Assert
		Assert.True(doesExist);
		Assert.False(doesNotExist);
	}

	[Fact]
	public void DoesArchiveGpgFileExist_TestFileExistsAndDoesNotExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		if (!System.IO.Directory.Exists(customerToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(customerToPrintContractor.ArchiveFolder);

		if (System.IO.File.Exists(customerToPrintContractor.ArchiveGpgFileFullPath))
		{
			File.Delete(customerToPrintContractor.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(customerToPrintContractor.ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doesExist = customerToPrintContractor.DoesArchiveGpgFileExist();

		if (System.IO.File.Exists(customerToPrintContractor.ArchiveGpgFileFullPath))
		{
			File.Delete(customerToPrintContractor.ArchiveGpgFileFullPath);
		}

		bool doesNotExist = customerToPrintContractor.DoesArchiveGpgFileExist();


		//Assert
		Assert.True(doesExist);
		Assert.False(doesNotExist);
	}

	[Fact]
	public void MoveArchiveFileToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		//Act
		_ = customerToPrintContractor.MoveArchiveFileToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerToPrintContractor.ArchiveFileFullPath}"
			&& request.DestinationFolder == $"{customerToPrintContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveArchiveFileToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		//Act
		_ = customerToPrintContractor.MoveArchiveFileToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerToPrintContractor.ArchiveFileFullPath}"
			&& request.DestinationFolder == $"{customerToPrintContractor.ArchiveFailedFolder}"), 
			CancellationToken.None), Times.Once);
		
	}

	[Fact]
	public void MoveArchiveGpgFileToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);
		
		//Act
		_ = customerToPrintContractor.MoveArchiveGpgFileToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerToPrintContractor.ArchiveGpgFileFullPath}"
			&& request.DestinationFolder == $"{customerToPrintContractor.ArchiveProcessedFolder}"), 
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveArchiveGpgFileToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		//Act
		_ = customerToPrintContractor.MoveArchiveGpgFileToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerToPrintContractor.ArchiveGpgFileFullPath}"
			&& request.DestinationFolder == $"{customerToPrintContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveGpgFileToDataTransferFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		//Act
		_ = customerToPrintContractor.MoveGpgFileToDataTransferFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<CopyFileCommand>(request =>
			request.SourceFile == $"{customerToPrintContractor.ArchiveGpgFileFullPath}"
			&& request.DestinationFolder == $"{customerToPrintContractor.DataTransferFolderBasePath}"),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public void EncryptFile_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersToPrintContractor customerToPrintContractor = new(mock.Object);

		//Act
		_ = customerToPrintContractor.EncryptFile();

		//Assert
		mock.Verify(g => g.Send(It.Is<EncryptFileCommand>(request =>
			request.InputFileLocation == $"{customerToPrintContractor.ArchiveFileFullPath}"
			&& request.OutputFileLocation == $"{customerToPrintContractor.ArchiveGpgFileFullPath}"
			&& request.PublicKeyName == GpgPublicKeyName), CancellationToken.None), Times.Once);
	}
}