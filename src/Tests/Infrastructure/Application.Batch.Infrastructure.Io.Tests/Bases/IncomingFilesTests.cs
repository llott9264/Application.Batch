using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Models;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Gpg.MediatR;
using Utilities.IoOperations.MediatR.File.MoveFile;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Infrastructure.Io.Tests.Bases;

public class IncomingFilesTests
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
	public void MoveArchiveFilesToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		_ = revokesFromContractor.MoveArchiveFilesToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{revokesFromContractor.Files[i].ArchiveFileFullPath}"
				&& request.DestinationFolder == $"{revokesFromContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveFilesToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		_ = revokesFromContractor.MoveArchiveFilesToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{revokesFromContractor.Files[i].ArchiveFileFullPath}"
				&& request.DestinationFolder == $"{revokesFromContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveGpgFilesToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		_ = revokesFromContractor.MoveArchiveGpgFilesToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{revokesFromContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{revokesFromContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveGpgFilesToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		_ = revokesFromContractor.MoveArchiveGpgFilesToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{revokesFromContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{revokesFromContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void DecryptFile_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		_ = revokesFromContractor.DecryptFiles();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<DecryptFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<DecryptFileCommand>(request =>
				request.InputFileLocation == $"{revokesFromContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.OutputFileLocation == $"{revokesFromContractor.Files[i].ArchiveFileFullPath}"
				&& request.PrivateKeyName == GpgPrivateKeyName
				&& request.PrivateKeyPassword == GpgPrivateKeyPassword), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void GetArchiveFileFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		string archiveFileFullPath = revokesFromContractor.GetArchiveFileFullPath("File1.txt");

		//Assert
		Assert.True(archiveFileFullPath == $"{revokesFromContractor.ArchiveFolder}File1.txt");
	}

	[Fact]
	public void GetArchiveGpgFileFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		string archiveGpgFileFullPath = revokesFromContractor.GetArchiveGpgFileFullPath("File1.txt");

		//Assert
		Assert.True(archiveGpgFileFullPath == $"{revokesFromContractor.ArchiveFolder}File1.txt");
	}

	[Fact]
	public void DataTransferGpgFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		//Act
		string archiveGpgFileFullPath = revokesFromContractor.DataTransferGpgFullPath("File1.txt");

		//Assert
		Assert.True(archiveGpgFileFullPath == $"{revokesFromContractor.DataTransferFolderBasePath}File1.txt");
	}

	[Fact]
	public async Task MoveGpgFilesToArchiveFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("File1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("File3.txt.gpg");

		//Act
		bool isSuccessful = await revokesFromContractor.MoveGpgFilesToArchiveFolder();

		//Assert
		Assert.True(isSuccessful);
		mock.Verify(g => g.Send(It.IsAny<CreateLogCommand>(), CancellationToken.None), Times.Exactly(3));
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "Begin copying gpg files to archive transfer."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "Successfully copied gpg files to archive folder."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "End copying gpg files to archive folder."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{revokesFromContractor.Files[i].DataTransferGpgFileFullPath}"
				&& request.DestinationFolder == $"{revokesFromContractor.ArchiveFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void DoArchiveGpgFilesExist_AllFilesExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("GpgFileExist1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("GpgFileExist2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("GpgFileExist3.txt.gpg");

		if (!System.IO.Directory.Exists(revokesFromContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);

		foreach (DecryptionFileDto file in revokesFromContractor.Files.Where(file => File.Exists(file.ArchiveGpgFileFullPath)))
		{
			File.Delete(file.ArchiveGpgFileFullPath);
		}

		foreach (DecryptionFileDto file in revokesFromContractor.Files)
		{
			using (StreamWriter writer = new(file.ArchiveGpgFileFullPath))
			{
				writer.WriteLine("Hello World!");
			}
		}

		//Act
		bool doExist = revokesFromContractor.DoArchiveGpgFilesExist();

		//Assert
		Assert.True(doExist);
	}

	[Fact]
	public void DoArchiveGpgFilesExist_OneFileDoesNotExist_ReturnsFalse()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("GpgFileDoesNotExist1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("GpgFileDoesNotExist2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("GpgFileDoesNotExist3.txt.gpg");

		if (!System.IO.Directory.Exists(revokesFromContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);

		foreach (DecryptionFileDto file in revokesFromContractor.Files.Where(file => File.Exists(file.ArchiveGpgFileFullPath)))
		{
			File.Delete(file.ArchiveGpgFileFullPath);
		}


		using (StreamWriter writer = new(revokesFromContractor.Files[0].ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		using (StreamWriter writer = new(revokesFromContractor.Files[1].ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doExist = revokesFromContractor.DoArchiveGpgFilesExist();

		//Assert
		Assert.False(doExist);
	}

	[Fact]
	public void DoArchiveFilesExist_AllFilesExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("ArchiveFileExist1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("ArchiveFileExistFile2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("ArchiveFileExistFile3.txt.gpg");

		if (!System.IO.Directory.Exists(revokesFromContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);

		foreach (DecryptionFileDto file in revokesFromContractor.Files.Where(file => File.Exists(file.ArchiveFileFullPath)))
		{
			File.Delete(file.ArchiveFileFullPath);
		}

		foreach (DecryptionFileDto file in revokesFromContractor.Files)
		{
			using (StreamWriter writer = new(file.ArchiveFileFullPath))
			{
				writer.WriteLine("Hello World!");
			}
		}

		//Act
		bool doExist = revokesFromContractor.DoArchiveFilesExist();

		//Assert
		Assert.True(doExist);
	}

	[Fact]
	public void DoArchiveFilesExist_OneFileDoesNotExist_ReturnsFalse()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());
		revokesFromContractor.AddFileToDecrypt("ArchiveFileDoesNotExist1.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("ArchiveFileDoesNotExist2.txt.gpg");
		revokesFromContractor.AddFileToDecrypt("ArchiveFileDoesNotExist3.txt.gpg");

		if (!System.IO.Directory.Exists(revokesFromContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(revokesFromContractor.ArchiveFolder);

		foreach (DecryptionFileDto file in revokesFromContractor.Files.Where(file => File.Exists(file.ArchiveFileFullPath)))
		{
			File.Delete(file.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(revokesFromContractor.Files[0].ArchiveFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		using (StreamWriter writer = new((revokesFromContractor.Files[1].ArchiveFileFullPath)))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doExist = revokesFromContractor.DoArchiveFilesExist();

		//Assert
		Assert.False(doExist);
	}

	[Fact]
	public void GetGpgFilesInDataTransferFolder_DecryptionFileDtoSetCorrectly_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RevokesFromContractor revokesFromContractor = new(mock.Object, GetMapper());

		if (!System.IO.Directory.Exists(revokesFromContractor.DataTransferFolderBasePath))
			System.IO.Directory.CreateDirectory(revokesFromContractor.DataTransferFolderBasePath);

		if (System.IO.File.Exists($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest1.txt.gpg"))
			File.Delete($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest1.txt.gpg");

		if (System.IO.File.Exists($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest2.txt.gpg"))
			File.Delete($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest2.txt.gpg");

		using (StreamWriter writer = new($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest1.txt.gpg"))
		{
			writer.WriteLine("Hello World!");
		}

		using (StreamWriter writer = new($"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest2.txt.gpg"))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		revokesFromContractor.GetGpgFilesInDataTransferFolder();

		//Assert
		Assert.True(revokesFromContractor.Files.Count == 2);

		DecryptionFileDto file1 = revokesFromContractor.Files[0];
		DecryptionFileDto file2 = revokesFromContractor.Files[1];

		Assert.True(file1.ArchiveGpgFileFullPath == $"{revokesFromContractor.ArchiveFolder}GpgFileTransferTest1.txt.gpg");
		Assert.True(file1.ArchiveFileFullPath == $"{revokesFromContractor.ArchiveFolder}GpgFileTransferTest1.txt");
		Assert.True(file1.DataTransferGpgFileFullPath == $"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest1.txt.gpg");

		Assert.True(file2.ArchiveGpgFileFullPath == $"{revokesFromContractor.ArchiveFolder}GpgFileTransferTest2.txt.gpg");
		Assert.True(file2.ArchiveFileFullPath == $"{revokesFromContractor.ArchiveFolder}GpgFileTransferTest2.txt");
		Assert.True(file2.DataTransferGpgFileFullPath == $"{revokesFromContractor.DataTransferFolderBasePath}GpgFileTransferTest2.txt.gpg");
	}
}
