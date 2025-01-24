using MediatR;

namespace Application.Batch.Core.Application.Features.Logger;

public class CreateLogEvent : IRequest
{
	public string Message { get; set; } = string.Empty;
	public Enums.Logger.Type MessageType { get; set; }

}