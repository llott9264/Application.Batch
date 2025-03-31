using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;
using Application.Batch.Core.Application.Features.Workflows.WorkflowRunner.Commands;
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
		WorkflowRunnerCommand request = new((WorkflowName)999);
		WorkflowRunnerCommandHandler handler = new(mock.Object);
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
		WorkflowRunnerCommand request = new(WorkflowName.CustomersToPrintContractor);
		WorkflowRunnerCommandHandler handler = new(mock.Object);
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
		WorkflowRunnerCommand request = new(WorkflowName.CustomersFromContractor);
		WorkflowRunnerCommandHandler handler = new(mock.Object);
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
		WorkflowRunnerCommand request = new(WorkflowName.RenewalsToPrintContractor);
		WorkflowRunnerCommandHandler handler = new(mock.Object);
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
		WorkflowRunnerCommand request = new(WorkflowName.RevokesFromContractor);
		WorkflowRunnerCommandHandler handler = new(mock.Object);
		//Act
		_ = handler.Handle(request, CancellationToken.None);
		//Assert
		mock.Verify(m => m.Send(It.IsAny<Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow.ProcessWorkflowCommand>(), It.IsAny<CancellationToken>()), Times.Once);
		mock.VerifyNoOtherCalls();
	}
}