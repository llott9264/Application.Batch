using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Pdf;
using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Core.Application.Features.Workflows.RenewalsToPrintContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Application.Models;
using Application.Batch.Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Moq;
using System.Linq.Expressions;
using Utilities.Configuration.MediatR;
using Utilities.Logging.EventLog.MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application.Tests.Workflows;

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

	private static Mock<IRenewalsToPrintContractor> GetMockWorkflow()
	{
		Mock<IRenewalsToPrintContractor> mock = new();
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

		List<EncryptionFileDto> filesToEncrypt =
			[new(ArchiveFolderBasePath, DataTransferFolderBasePath, "File1.txt")];

		Mock<IMediator> mockMediator = GetMockMediator();

		Mock<IUnitOfWork> mockUnitOfWork = GetMockUnitOfWork();
		Mock<ICustomerRepository> customerRepository = new();
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(m => m.Files).Returns(filesToEncrypt);
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockWorkflow.Verify(g => g.CopyGpgFilesToDataTransferFolder(), Times.Once);
		mockWorkflow.Verify(g => g.MoveArchiveFilesToProcessedFolder(), Times.Once);
		mockWorkflow.Verify(g => g.MoveArchiveGpgFilesToProcessedFolder(), Times.Once);
		
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
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
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("No customers were found. Files not generated.")
				&& request.LogType == LogType.Warning),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_WriteFilesReturnsFalse_MethodCompletesSuccessfully()
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
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(false);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to write files for Renewals To Print Contractor.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_DoArchiveFilesExistReturnsFalse_MethodCompletesSuccessfully()
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
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(false);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to find files for Renewals To Print Contractor.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}

	[Fact]
	public async Task CallHandleMethod_DoArchiveGpgFilesExistReturnsFalse_MethodCompletesSuccessfully()
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
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(false);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(true);

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Failed to encrypt file for Renewals To Print Contractor.")
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
		customerRepository.Setup(m => m.GetCustomersIncludeAddresses()).Returns(customers);
		mockUnitOfWork.Setup(m => m.Customers).Returns(customerRepository.Object);

		Mock<IRenewalsToPrintContractor> mockWorkflow = GetMockWorkflow();
		mockWorkflow.Setup(w => w.DoArchiveGpgFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.DoArchiveFilesExist()).Returns(true);
		mockWorkflow.Setup(w => w.WriteFiles(customers)).Returns(true);
		mockWorkflow.Setup(w => w.EncryptFiles()).Throws(new Exception());

		ProcessWorkflowCommand command = new();
		ProcessWorkflowCommandHandler handler = new(mockMediator.Object, mockWorkflow.Object, mockUnitOfWork.Object);

		//Act
		await handler.Handle(command, CancellationToken.None);

		//Assert
		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Begin generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("Error occurred generating file for Renewals To Print Contractor.")
				&& request.LogType == LogType.Error),
			CancellationToken.None), Times.Once);

		mockMediator.Verify(g => g.Send(It.Is<CreateLogCommand>(request =>
				request.Message.Contains("End generating file for Customer List.")
				&& request.LogType == LogType.Information),
			CancellationToken.None), Times.Once);
	}
}