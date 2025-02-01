using MediatR;
using Utilities.Gpg;

namespace Application.Batch.Core.Application.Features.Utilities.Gpg.Commands;

public class EncryptFileHandler(IGpg gpg) : IRequestHandler<EncryptFileCommand>
{
	public async Task Handle(EncryptFileCommand request, CancellationToken cancellationToken)
	{
		await gpg.EncryptFileAsync(request.InputFileLocation, request.OutputFileLocation, request.PublicKeyName);
	}
}