using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;
using MediatR;
using Moq;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Tests.Workflows;

public class ApplicationWorkflowTests
{
	[Fact]
	public void ProcessWorkflowHandler_InvalidWorkflowName_CallsCreateLogCommand()
	{
		//Arrange
		Mock<IMediator> mock = new();
		ProcessWorkflowCommand request = new((WorkflowName)999);
		ProcessWorkflowCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<CreateLogCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}

	[Fact]
	public void ProcessWorkflowHandler_CustomersToPrintContractor_CallsProcessWorkflowCommand()
	{
		//Arrange
		Mock<IMediator> mock = new();
		ProcessWorkflowCommand request = new(WorkflowName.CustomersToPrintContractor);
		ProcessWorkflowCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<Features.Workflows.CustomersToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}

	[Fact]
	public void ProcessWorkflowHandler_CustomersFromContractor_CallsProcessWorkflowCommand()
	{
		//Arrange
		Mock<IMediator> mock = new();
		ProcessWorkflowCommand request = new(WorkflowName.CustomersFromContractor);
		ProcessWorkflowCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}

	[Fact]
	public void ProcessWorkflowHandler_RenewalsToPrintContractor_CallsProcessWorkflowCommand()
	{
		//Arrange
		Mock<IMediator> mock = new();
		ProcessWorkflowCommand request = new(WorkflowName.RenewalsToPrintContractor);
		ProcessWorkflowCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<Features.Workflows.RenewalsToPrintContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}

	[Fact]
	public void ProcessWorkflowHandler_RevokesFromContractor_CallsProcessWorkflowCommand()
	{
		//Arrange
		Mock<IMediator> mock = new();
		ProcessWorkflowCommand request = new(WorkflowName.RevokesFromContractor);
		ProcessWorkflowCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}
}