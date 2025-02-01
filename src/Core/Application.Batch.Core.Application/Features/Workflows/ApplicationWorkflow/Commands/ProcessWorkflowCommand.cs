using Application.Batch.Core.Application.Enums;
using MediatR;

namespace Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;

public class ProcessWorkflowCommand(WorkflowName workflowName) : IRequest
{
	public WorkflowName WorkflowName { get; } = workflowName;
}