using Application.Batch.Core.Application.Enums;
using MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application.Features.Log.Commands;

public class CreateLogHandler(ILog logger) : IRequestHandler<CreateLogCommand>
{
	public Task Handle(CreateLogCommand request, CancellationToken cancellationToken)
	{
		switch (request.LogType)
		{
			case LogType.Debug:
				logger.Debug(request.Message);
				break;
			case LogType.Information:
				logger.Information(request.Message);
				break;
			case LogType.Warning:
				logger.Warning(request.Message);
				break;
			case LogType.Error:
				logger.Error(request.Message);
				break;
			default:
				logger.Error($"Log Type {request.LogType} not found.  Could not write message to Log:  {request.Message}");
				break;
		}
		

		return Task.CompletedTask;
	}
}