using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Models;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;
using ProcessWorkflowCommand = Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand;
using ProcessWorkflowCommandHandler = Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommandHandler;
using Application.Batch.Core.Domain.Entities;
using System.Linq.Expressions;

namespace Application.Batch.Core.Application.Tests.Workflows;

public class RevokesFromContractorTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPrivateKey.asc";
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

	private static Mock<IRevokesFromContractor> GetMockWorkflow()
	{
		Mock<IRevokesFromContractor> mock = new();
		return mock;
	}

	private static Mock<IUnitOfWork> GetMockUnitOfWork()
	{
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Public",
				SocialSecurityNumber = "1235456789",
				IsRevoked = false
			}
		];

		Mock<ICustomerRepository> mockCustomerRepository = new();
		mockCustomerRepository.Setup(m => m.Find(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(customers);

		Mock<IUnitOfWork> mock = new();
		mock.Setup(m => m.Customers).Returns(mockCustomerRepository.Object);
		return mock;
	}

	[Fact]
	public async Task CallHandleMethod_UpdateRevoke_MethodCompletesSuccessfully()
	{
		//Arrange
		List<RevokeViewModel> revokes =
		[
			new()
			{
				SocialSecurityNumber = "123456789",
				IsRevoked = true
			}
		];

		List<DecryptionFileDto> filesToDecrypt =
			[new(ArchiveFolderBasePath, DataTransferFolderBasePath, "GpgFile1.txt.gpg")];

		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();

		Mock<IRevokesFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(m => m.Files).Returns(filesToDecrypt);
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.ReadFiles()).Returns(revokes);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin revoking Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Successfully imported revokes data.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End revoking customer.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_FailsToMoveGpgFileToArchiveFolder()
	{
		

		List<DecryptionFileDto> filesToDecrypt =
			[new(ArchiveFolderBasePath, DataTransferFolderBasePath, "GpgFile1.txt.gpg")];

		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IRevokesFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(m => m.Files).Returns(filesToDecrypt);
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, GetMockUnitOfWork().Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin revoking Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to move gpg files to archive folder.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End revoking customer.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_FailsToDecryptFile()
	{


		List<DecryptionFileDto> filesToDecrypt =
			[new(ArchiveFolderBasePath, DataTransferFolderBasePath, "GpgFile1.txt.gpg")];

		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IRevokesFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(m => m.Files).Returns(filesToDecrypt);
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, GetMockUnitOfWork().Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin revoking Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to decrypt files.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End revoking customer.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_FilesSetToNull_ThrowsException()
	{
		//Arrange
		List<RevokeViewModel> revokes =
		[
			new()
			{
				SocialSecurityNumber = "123456789",
				IsRevoked = true
			}
		];

		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();

		Mock<IRevokesFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.ReadFiles()).Returns(revokes);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin revoking Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Error occurred revoking customer.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End revoking customer.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_NoFiles_MethodCompletesSuccessfully()
	{
		//Arrange
		List<RevokeViewModel> revokes =
		[
			new()
			{
				SocialSecurityNumber = "123456789",
				IsRevoked = true
			}
		];

		List<DecryptionFileDto> filesToDecrypt = new();

		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();

		Mock<IRevokesFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(m => m.Files).Returns(filesToDecrypt);
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.ReadFiles()).Returns(revokes);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin revoking Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("No files to decrypt.")
				&& request.LogType == LogType.Warning),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End revoking customer.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}
}