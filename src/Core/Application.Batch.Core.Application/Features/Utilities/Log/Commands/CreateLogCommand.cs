using Application.Batch.Core.Application.Enums;
using MediatR;

namespace Application.Batch.Core.Application.Features.Utilities.Log.Commands;

public class CreateLogCommand(string message, LogType logType) : IRequest
{
	public string Message { get; set; } = message;
	public LogType LogType { get; set; } = logType;
}