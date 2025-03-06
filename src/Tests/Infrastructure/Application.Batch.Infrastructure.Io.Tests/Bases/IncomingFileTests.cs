using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Gpg.MediatR;
using Utilities.IoOperations.MediatR.File.CopyFile;
using Utilities.IoOperations.MediatR.File.MoveFile;

namespace Application.Batch.Infrastructure.Io.Tests.Bases;

public class IncomingFileTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPublicKey.asc";
	private const string GpgPrivateKeyPassword = "password";

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:PrivateKeyName"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:PrivateKeyPassword"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
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
	public void DoesArchiveFileExist_TestFileExistsAndDoesNotExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		if (!System.IO.Directory.Exists(customerFromPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(customerFromPrintContractor.ArchiveFolder);

		if (System.IO.File.Exists(customerFromPrintContractor.ArchiveFileFullPath))
		{
			File.Delete(customerFromPrintContractor.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(customerFromPrintContractor.ArchiveFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doesExist = customerFromPrintContractor.DoesArchiveFileExist();

		if (System.IO.File.Exists(customerFromPrintContractor.ArchiveFileFullPath))
		{
			File.Delete(customerFromPrintContractor.ArchiveFileFullPath);
		}

		bool doesNotExist = customerFromPrintContractor.DoesArchiveFileExist();


		//Assert
		Assert.True(doesExist);
		Assert.False(doesNotExist);
	}

	[Fact]
	public void DoesArchiveGpgFileExist_TessFileExistsAndDoesNotExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		if (!System.IO.Directory.Exists(customerFromPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(customerFromPrintContractor.ArchiveFolder);

		if (System.IO.File.Exists(customerFromPrintContractor.ArchiveGpgFileFullPath))
		{
			File.Delete(customerFromPrintContractor.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(customerFromPrintContractor.ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doesExist = customerFromPrintContractor.DoesArchiveGpgFileExist();

		if (System.IO.File.Exists(customerFromPrintContractor.ArchiveGpgFileFullPath))
		{
			File.Delete(customerFromPrintContractor.ArchiveGpgFileFullPath);
		}

		bool doesNotExist = customerFromPrintContractor.DoesArchiveGpgFileExist();


		//Assert
		Assert.True(doesExist);
		Assert.False(doesNotExist);
	}

	[Fact]
	public void MoveArchiveFileToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.MoveArchiveFileToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerFromPrintContractor.ArchiveFileFullPath}"
			&& request.DestinationFolder == $"{customerFromPrintContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveArchiveFileToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.MoveArchiveFileToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{customerFromPrintContractor.ArchiveFileFullPath}"
				&& request.DestinationFolder == $"{customerFromPrintContractor.ArchiveFailedFolder}"),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveArchiveGpgFileToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.MoveArchiveGpgFileToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{customerFromPrintContractor.ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{customerFromPrintContractor.ArchiveProcessedFolder}"),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveArchiveGpgFileToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.MoveArchiveGpgFileToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
			request.SourceFile == $"{customerFromPrintContractor.ArchiveGpgFileFullPath}"
			&& request.DestinationFolder == $"{customerFromPrintContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
	}

	[Fact]
	public void MoveToGpgFileToArchiveFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.MoveToGpgFileToArchiveFolder();

		//Assert
		mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{customerFromPrintContractor.DataTransferGpgFullPath}"
				&& request.DestinationFolder == $"{customerFromPrintContractor.ArchiveFolder}"),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public void EncryptFile_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		CustomersFromContractor customerFromPrintContractor = new(mock.Object, GetMapper());

		//Act
		_ = customerFromPrintContractor.DecryptFile();

		//Assert
		mock.Verify(g => g.Send(It.Is<DecryptFileCommand>(request =>
			request.InputFileLocation == $"{customerFromPrintContractor.ArchiveGpgFileFullPath}"
			&& request.OutputFileLocation == $"{customerFromPrintContractor.ArchiveFileFullPath}"
			&& request.PrivateKeyName == GpgPrivateKeyName
			&& request.PrivateKeyPassword == GpgPrivateKeyPassword), CancellationToken.None), Times.Once);
	}
}