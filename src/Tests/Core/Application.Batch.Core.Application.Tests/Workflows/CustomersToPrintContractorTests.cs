using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Features.Workflows.CustomersToPrintContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Domain.Entities;
using Castle.Components.DictionaryAdapter;
using MediatR;
using Moq;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application.Tests.Workflows;

public class CustomersToPrintContractorTests
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

	private static Mock<ICustomerToPrintContractor> GetMockWorkflow()
	{
		Mock<ICustomerToPrintContractor> mock = new();
		return mock;
	}

	private static Mock<IUnitOfWork> GetMockUnitOfWork()
	{
		Mock<IUnitOfWork> mock = new();
		return mock;
	}

	[Fact]
	public async Task CallHandleMethod_MethodCompletesSuccessfully()
	{
		//Arrange
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Smith",
				SocialSecurityNumber = "123456789",
				IsRevoked = false
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetAll()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<ICustomerToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFile(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFileToDataTransferFolder(), Times.Once);
		mockWorkflow.Verify(g => g.MoveArchiveFileToProcessedFolder(), Times.Once);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFileToProcessedFolder(), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Successfully completed workflow.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_NoCustomersFound_MethodCompletesSuccessfully()
	{
		//Arrange
		List<Customer> customers = new();
		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetAll()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<ICustomerToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFile(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFileToDataTransferFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveFileToProcessedFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFileToProcessedFolder(), Times.Never);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("No customers were found.")
				&& request.LogType == LogType.Warning),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_WriteFileReturnsFalse_MethodCompletesSuccessfully()
	{
		//Arrange
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Smith",
				SocialSecurityNumber = "123456789",
				IsRevoked = false
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetAll()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<ICustomerToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFile(customers)).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFileToDataTransferFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveFileToProcessedFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFileToProcessedFolder(), Times.Never);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to write Customer file.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_DoesArchiveGpgFileExistReturnsFalse_MethodCompletesSuccessfully()
	{
		//Arrange
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Smith",
				SocialSecurityNumber = "123456789",
				IsRevoked = false
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetAll()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<ICustomerToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(false);
		mockWorkflow.Setup(w => w.WriteFile(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFileToDataTransferFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveFileToProcessedFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFileToProcessedFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveFileToFailedFolder(), Times.Once);
		

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to encrypt Customer file.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_ThrowsException()
	{
		//Arrange
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Smith",
				SocialSecurityNumber = "123456789",
				IsRevoked = false
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetAll()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<ICustomerToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFile(customers)).Returns(true);
		mockWorkflow.Setup(w => w.EncryptFile()).Throws(new Exception());

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFileToDataTransferFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveFileToProcessedFolder(), Times.Never);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFileToProcessedFolder(), Times.Never);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Error occurred generating Customer List.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}
}