using MediatR;

namespace Application.Batch.Core.Application.Features.Utilities.Configuration.Queries;

public class GetConfigurationByKeyQuery(string key) : IRequest<string>
{
	public string Key { get; } = key;
}