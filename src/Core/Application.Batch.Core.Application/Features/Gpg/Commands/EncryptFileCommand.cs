using MediatR;

namespace Application.Batch.Core.Application.Features.Gpg.Commands;

public class EncryptFileCommand(string inputFileLocation, string outputFileLocation, string publicKeyName) : IRequest
{
	public string InputFileLocation { get; } = inputFileLocation;
	public string OutputFileLocation { get; } = outputFileLocation;
	public string PublicKeyName { get; } = publicKeyName;
}