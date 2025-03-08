using Application.Batch.Core.Application.Enums;
using Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;
using MediatR;
using Moq;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.ConsoleApp.Tests
{
	public class WorkflowRunnerTests
	{
		private static Mock<IMediator> GetMockMediator()
		{
			Mock<IMediator> mock = new();
			mock.Setup(m => m.Send(It.IsAny<ProcessWorkflowCommand>(), CancellationToken.None))
				.Returns(Task.CompletedTask);
			return mock;
		}
		
		[Fact]
		public async Task RunAsync_WithValidWorkflowName_SendsProcessWorkflowCommand()
		{
			// Arrange
			Mock<IMediator> mockMediator = GetMockMediator();
			WorkflowRunner runner = new(mockMediator.Object);
			string[] args = ["CustomersToPrintContractor"];
			
			// Act
			await runner.RunAsync(args);

			// Assert
			mockMediator.Verify(m => m.Send(It.Is<ProcessWorkflowCommand>(
				cmd => cmd.WorkflowName == WorkflowName.CustomersToPrintContractor),
				CancellationToken.None), Times.Once);

			mockMediator.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task RunAsync_WithInvalidWorkflowName_SendsCreateLogCommand()
		{
			// Arrange
			Mock<IMediator> mockMediator = GetMockMediator();
			WorkflowRunner runner = new(mockMediator.Object);
			string[] args = ["Invalid"];

			// Act
			await runner.RunAsync(args);

			// Assert
			mockMediator.Verify(m => m.Send(It.Is<CreateLogCommand>(
					cmd => cmd.Message.Contains("Invalid parameter:")
						&& cmd.LogType == LogType.Error),
				CancellationToken.None), Times.Once);

			mockMediator.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task RunAsync_WithNoArgs_UsesDefaultWorkflow()
		{
			// Arrange
			Mock<IMediator> mockMediator = GetMockMediator();
			WorkflowRunner runner = new(mockMediator.Object);
			string[] args = [];
	
			// Act
			await runner.RunAsync(args);

			// Assert
			mockMediator.Verify(m => m.Send(It.Is<CreateLogCommand>(
					cmd => cmd.Message.Contains("Invalid parameter:")
					       && cmd.LogType == LogType.Error),
				CancellationToken.None), Times.Once);
		}

		[Fact]
		public async Task Constructor_WithNullMediator_ThrowsArgumentNullException()
		{
			// Arrange
			WorkflowRunner runner = new(null);
			string[] args = [];

			// Act & Assert
			Exception ex = await Assert.ThrowsAsync<ArgumentNullException>(() => runner.RunAsync(args));
			Assert.Contains("Value cannot be null. (Parameter 'mediator')", ex.Message);
		}

		[Fact]
		public async Task RunAsync_SetsEnvironmentVariable()
		{
			// Arrange
			Mock<IMediator> mock = GetMockMediator();
			mock.Setup(m => m.Send(It.IsAny<ProcessWorkflowCommand>(), CancellationToken.None)).Returns(Task.CompletedTask);
			WorkflowRunner runner = new(mock.Object);
			string[] args = ["CustomersToPrintContractor"];
			Environment.SetEnvironmentVariable("ITEXT_BOUNCY_CASTLE_FACTORY_NAME", null); // Clear it first

			// Act
			await runner.RunAsync(args);

			// Assert
			Assert.Equal("bouncy-castle", Environment.GetEnvironmentVariable("ITEXT_BOUNCY_CASTLE_FACTORY_NAME"));
		}
	}
}