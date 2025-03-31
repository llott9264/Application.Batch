using Application.Batch.Core.Application.Contracts.Pdf;
using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using AutoMapper;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Gpg.MediatR;
using Utilities.IoOperations.MediatR.File.MoveFile;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;
using Application.Batch.Core.Application.Models;
using Utilities.IoOperations.MediatR.File.CopyFile;

namespace Application.Batch.Infrastructure.Io.Tests.Bases;

public class OutgoingFilesTests
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

	private static IMapper GetMapper()
	{
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		Mapper mapper = new(configuration);
		return mapper;
	}

	private Mock<IRenewalsToPrintContractorPdf> GetMockPdf()
	{
		Mock<IRenewalsToPrintContractorPdf> mock = new();
		mock.Setup(m => m.CreatePdf(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Customer[]>()));
		return mock;
	}

	[Fact]
	public void MoveArchiveFilesToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt.gpg");

		//Act
		_ = renewalsToPrintContractor.MoveArchiveFilesToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{renewalsToPrintContractor.Files[i].ArchiveFileFullPath}"
				&& request.DestinationFolder == $"{renewalsToPrintContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveFilesToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt.gpg");

		//Act
		_ = renewalsToPrintContractor.MoveArchiveFilesToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{renewalsToPrintContractor.Files[i].ArchiveFileFullPath}"
				&& request.DestinationFolder == $"{renewalsToPrintContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveGpgFilesToProcessedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt.gpg");

		//Act
		_ = renewalsToPrintContractor.MoveArchiveGpgFilesToProcessedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{renewalsToPrintContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{renewalsToPrintContractor.ArchiveProcessedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void MoveArchiveGpgFilesToFailedFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt.gpg");

		//Act
		_ = renewalsToPrintContractor.MoveArchiveGpgFilesToFailedFolder();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<MoveFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<MoveFileCommand>(request =>
				request.SourceFile == $"{renewalsToPrintContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{renewalsToPrintContractor.ArchiveFailedFolder}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void EncryptFile_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt");

		//Act
		_ = renewalsToPrintContractor.EncryptFiles();

		//Assert
		mock.Verify(g => g.Send(It.IsAny<EncryptFileCommand>(), CancellationToken.None), Times.Exactly(3));

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<EncryptFileCommand>(request =>
				request.InputFileLocation == $"{renewalsToPrintContractor.Files[i].ArchiveFileFullPath}"
				&& request.OutputFileLocation == $"{renewalsToPrintContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.PublicKeyName == GpgPublicKeyName), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void GetArchiveFileFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);

		//Act
		string archiveFileFullPath = renewalsToPrintContractor.GetArchiveFileFullPath("File1.txt");

		//Assert
		Assert.True(archiveFileFullPath == $"{renewalsToPrintContractor.ArchiveFolder}File1.txt");
	}

	[Fact]
	public void GetArchiveGpgFileFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);

		//Act
		string archiveGpgFileFullPath = renewalsToPrintContractor.GetArchiveGpgFileFullPath("File1.txt");

		//Assert
		Assert.True(archiveGpgFileFullPath == $"{renewalsToPrintContractor.ArchiveFolder}File1.txt");
	}

	[Fact]
	public void DataTransferGpgFullPath_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);

		//Act
		string archiveGpgFileFullPath = renewalsToPrintContractor.DataTransferGpgFullPath("File1.txt");

		//Assert
		Assert.True(archiveGpgFileFullPath == $"{renewalsToPrintContractor.DataTransferFolderBasePath}File1.txt");
	}

	[Fact]
	public async Task CopyGpgFilesToDataTransferFolder_MethodCallsCorrectly()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("File1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("File3.txt.gpg");

		//Act
		bool isSuccessful = await renewalsToPrintContractor.CopyGpgFilesToDataTransferFolder();

		//Assert
		Assert.True(isSuccessful);
		mock.Verify(g => g.Send(It.IsAny<CreateLogCommand>(), CancellationToken.None), Times.Exactly(3));
		mock.Verify(g => g.Send(It.IsAny<CopyFileCommand>(), CancellationToken.None), Times.Exactly(3));

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "Begin copying gpg files to data transfer."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "Successfully copied gpg files to data transfer folder."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		mock.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
			request.Message == "End copying gpg files to data transfer."
			&& request.LogType == LogType.Information), CancellationToken.None), Times.Once);

		for (int i = 0; i < 3; i++)
		{
			mock.Verify(g => g.Send(It.Is<CopyFileCommand>(request =>
				request.SourceFile == $"{renewalsToPrintContractor.Files[i].ArchiveGpgFileFullPath}"
				&& request.DestinationFolder == $"{renewalsToPrintContractor.DataTransferFolderBasePath}"), CancellationToken.None), Times.Once);
		}
	}

	[Fact]
	public void DoArchiveGpgFilesExist_AllFilesExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileExist1.txt");
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileExist2.txt");
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileExist3.txt");

		if (!System.IO.Directory.Exists(renewalsToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(renewalsToPrintContractor.ArchiveFolder);

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files.Where(file => File.Exists(file.ArchiveGpgFileFullPath)))
		{
			File.Delete(file.ArchiveGpgFileFullPath);
		}

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files)
		{
			using (StreamWriter writer = new(file.ArchiveGpgFileFullPath))
			{
				writer.WriteLine("Hello World!");
			}
		}

		//Act
		bool doExist = renewalsToPrintContractor.DoArchiveGpgFilesExist();

		//Assert
		Assert.True(doExist);
	}

	[Fact]
	public void DoArchiveGpgFilesExist_OneFileDoesNotExist_ReturnsFalse()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileDoesNotExist1.txt");
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileDoesNotExist2.txt");
		renewalsToPrintContractor.AddFileToEncrypt("GpgFileDoesNotExist3.txt");

		if (!System.IO.Directory.Exists(renewalsToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(renewalsToPrintContractor.ArchiveFolder);

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files.Where(file => File.Exists(file.ArchiveGpgFileFullPath)))
		{
			File.Delete(file.ArchiveGpgFileFullPath);
		}


		using (StreamWriter writer = new(renewalsToPrintContractor.Files[0].ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		using (StreamWriter writer = new(renewalsToPrintContractor.Files[1].ArchiveGpgFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doExist = renewalsToPrintContractor.DoArchiveGpgFilesExist();

		//Assert
		Assert.False(doExist);
	}

	[Fact]
	public void DoArchiveFilesExist_AllFilesExist_ReturnsTrue()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileExist1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileExistFile2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileExistFile3.txt.gpg");

		if (!System.IO.Directory.Exists(renewalsToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(renewalsToPrintContractor.ArchiveFolder);

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files.Where(file => File.Exists(file.ArchiveFileFullPath)))
		{
			File.Delete(file.ArchiveFileFullPath);
		}

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files)
		{
			using (StreamWriter writer = new(file.ArchiveFileFullPath))
			{
				writer.WriteLine("Hello World!");
			}
		}

		//Act
		bool doExist = renewalsToPrintContractor.DoArchiveFilesExist();

		//Assert
		Assert.True(doExist);
	}

	[Fact]
	public void DoArchiveFilesExist_OneFileDoesNotExist_ReturnsFalse()
	{
		//Arrange
		Mock<IMediator> mock = GetMockMediator();
		RenewalsToPrintContractor renewalsToPrintContractor = new(mock.Object, GetMockPdf().Object);
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileDoesNotExist1.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileDoesNotExist2.txt.gpg");
		renewalsToPrintContractor.AddFileToEncrypt("ArchiveFileDoesNotExist3.txt.gpg");

		if (!System.IO.Directory.Exists(renewalsToPrintContractor.ArchiveFolder))
			System.IO.Directory.CreateDirectory(renewalsToPrintContractor.ArchiveFolder);

		foreach (EncryptionFileDto file in renewalsToPrintContractor.Files.Where(file => File.Exists(file.ArchiveFileFullPath)))
		{
			File.Delete(file.ArchiveFileFullPath);
		}

		using (StreamWriter writer = new(renewalsToPrintContractor.Files[0].ArchiveFileFullPath))
		{
			writer.WriteLine("Hello World!");
		}

		using (StreamWriter writer = new((renewalsToPrintContractor.Files[1].ArchiveFileFullPath)))
		{
			writer.WriteLine("Hello World!");
		}

		//Act
		bool doExist = renewalsToPrintContractor.DoArchiveFilesExist();

		//Assert
		Assert.False(doExist);
	}
}