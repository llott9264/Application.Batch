using MediatR;
using Microsoft.Extensions.Configuration;

namespace Application.Batch.Core.Application.Features.Configuration.Queries;

public class GetConfigurationByKeyQueryHandler(IConfiguration configuration) : IRequestHandler<GetConfigurationByKeyQuery, string>
{

	public async Task<string> Handle(GetConfigurationByKeyQuery request, CancellationToken cancellationToken)
	{
		return configuration.GetValue<string>(request.Key);
	}
}