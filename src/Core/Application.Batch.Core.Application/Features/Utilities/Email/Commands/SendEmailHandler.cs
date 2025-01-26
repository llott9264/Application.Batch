using MediatR;
using Utilities.Email;

namespace Application.Batch.Core.Application.Features.Utilities.Email.Commands;

public class SendEmailHandler(IEmail email) : IRequestHandler<SendEmailCommand>
{
	public Task Handle(SendEmailCommand request, CancellationToken cancellationToken)
	{
		email.SendEmail(request.Subject, request.Body, request.Recipients, request.RecipientsCc, request.Delimiter);
		return Task.CompletedTask;
	}
}