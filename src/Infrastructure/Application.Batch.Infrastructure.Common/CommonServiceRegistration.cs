using Application.Batch.Core.Application.Contracts.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Common;

public static class CommonServiceRegistration
{

	public static IServiceCollection AddCommonServices(this IServiceCollection services)
	{
		services.AddScoped<IApplication, Application>();
		return services;
	}
	
}