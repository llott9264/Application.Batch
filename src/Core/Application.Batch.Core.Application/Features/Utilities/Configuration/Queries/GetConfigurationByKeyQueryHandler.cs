using Application.Batch.Core.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;

public class GetConfigurationByKeyQueryHandler(IConfiguration configuration) : IRequestHandler<GetConfigurationByKeyQuery, string>
{

	public Task<string> Handle(GetConfigurationByKeyQuery request, CancellationToken cancellationToken)
	{
		string? value = configuration.GetValue<string>(request.Key);

		if (string.IsNullOrWhiteSpace(value))
		{
			throw new NotFoundException("Configuration Value", request.Key);
		}

		return Task.FromResult(value);
	}
}