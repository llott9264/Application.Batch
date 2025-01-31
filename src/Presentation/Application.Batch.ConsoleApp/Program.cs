using Application.Batch.ConsoleApp;
using Application.Batch.Core.Application.Enums;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Application.Batch.Core.Application.Features.Utilities.Log.Commands;
using Application.Batch.Core.Application.Features.Workflows.ApplicationWorkflow.Commands;

IConfiguration configuration = new ConfigurationBuilder().GetConfiguration();
IHost host = StartupExtensions.BuildHost(configuration);
IMediator? mediator = host.Services.GetService<IMediator>();
string workFlowName = args.Length > 0 ? args[0] : "default";

if (mediator == null)
{
	return;
}

if (Enum.TryParse(workFlowName, out WorkflowName workflowEnum))
{
	
	await mediator.Send(new ProcessWorkflowCommand(workflowEnum));
}
else
{
	await mediator.Send(new CreateLogCommand($"Invalid parameter: {workFlowName}", LogType.Error));
}
