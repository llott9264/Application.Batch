using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Tests.Workflows;

public class CustomersFromContractorTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPrivateKey.asc";
	private const string GpgPrivateKeyPassword = "password";

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:PrivateKeyName"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:CustomersFromContractor:PrivateKeyPassword"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
		return mock;
	}

	private static Mock<ICustomersFromContractor> GetMockWorkflow()
	{
		Mock<ICustomersFromContractor> mock = new();
		return mock;
	}

	private static Mock<IUnitOfWork> GetMockUnitOfWorkForUpdate()
	{
		List<Customer> customers =
		[
			new()
			{
				FirstName = "John",
				LastName = "Public",
				SocialSecurityNumber = "1235456789"
			}
		];

		Mock<ICustomerRepository> mockCustomerRepository = new Mock<ICustomerRepository>();
		mockCustomerRepository.Setup(m => m.Find(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(customers);

		Mock<IUnitOfWork> mock = new();
		mock.Setup(m => m.Customers).Returns(mockCustomerRepository.Object);
		return mock;
	}

	private static Mock<IUnitOfWork> GetMockUnitOfWorkForAdd()
	{
		List<Customer> customers = new();

		Mock<ICustomerRepository> mockCustomerRepository = new Mock<ICustomerRepository>();
		mockCustomerRepository.Setup(m => m.Find(It.IsAny<Expression<Func<Customer, bool>>>())).Returns(customers);

		Mock<IUnitOfWork> mock = new();
		mock.Setup(m => m.Customers).Returns(mockCustomerRepository.Object);
		return mock;
	}

	[Fact]
	public async Task CallHandleMethod_UpdateCustomer_MethodCompletesSuccessfully()
	{
		//Arrange
		List<CustomerViewModel> customers =
		[
			new()
			{
				FirstName = "Joe",
				LastName = "Smith",
				SocialSecurityNumber = "123456789"
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWorkForUpdate();

		Mock<ICustomersFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoesArchiveFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.ReadFile()).Returns(Task.FromResult(customers));

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Successfully imported customer data.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_AddCustomer_MethodCompletesSuccessfully()
	{
		//Arrange
		List<CustomerViewModel> customers =
		[
			new()
			{
				FirstName = "Joe",
				LastName = "Smith",
				SocialSecurityNumber = "987654321"
			}
		];
		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWorkForAdd();

		Mock<ICustomersFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoesArchiveFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.ReadFile()).Returns(Task.FromResult(customers));

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Successfully imported customer data.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_FailsToMoveGpgFileToArchiveFolder()
	{
		//Arrange
		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<ICustomersFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, GetMockUnitOfWorkForUpdate().Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to move gpg file to archive folder.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_FailsToDecryptFile()
	{
		//Arrange
		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<ICustomersFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoesArchiveFileExist()).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, GetMockUnitOfWorkForUpdate().Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to decrypt file.")
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
		Mock<IMediator> mockMediator = GetMockMediator();
		Mock<ICustomersFromContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoesArchiveGpgFileExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoesArchiveFileExist()).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, GetMockUnitOfWorkForUpdate().Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
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
