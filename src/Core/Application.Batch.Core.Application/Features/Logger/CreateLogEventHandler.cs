using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Application.Exceptions;
using Application.Batch.Core.Application.Features.Customers.Commands.CreateCustomer;
using FluentValidation.Results;
using MediatR;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application.Features.Logger;

public class CreateLogEventHandler(ILog logger) : IRequestHandler<CreateLogEvent>
{
	public async Task Handle(CreateLogEvent request, CancellationToken cancellationToken)
	{
		CreateLogEventValidator validator = new();
		ValidationResult? validationResult = await validator.ValidateAsync(request, cancellationToken);

		if (validationResult.Errors.Count > 0)
		{
			throw new ValidationException(validationResult);
		}

		string message = request.Message;

		switch (request.MessageType)
		{
			case Enums.Logger.Type.Debug:
				logger.Debug(message);
				break;
			case Enums.Logger.Type.Information:
				logger.Information(message);
				break;
			case Enums.Logger.Type.Warning:
				logger.Warning(message);
				break;
			case Enums.Logger.Type.Error:
				logger.Error(message);
				break;
			default:
				logger.Error($"Could not write message due to Message type not found:  Message:  {message}");
				break;
		}
	}
}