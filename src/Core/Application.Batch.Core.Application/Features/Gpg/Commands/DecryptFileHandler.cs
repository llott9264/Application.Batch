using MediatR;
using Utilities.Gpg;

namespace Application.Batch.Core.Application.Features.Gpg.Commands;

public class DecryptFileHandler(IGpg gpg) : IRequestHandler<DecryptFileCommand>
{
	public async Task Handle(DecryptFileCommand request, CancellationToken cancellationToken)
	{
		await gpg.DecryptFileAsync(request.InputFileLocation, request.OutputFileLocation, request.PrivateKeyName, request.PrivateKeyPassword);
	}
}