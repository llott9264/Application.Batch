using Application.Batch.Core.Application.Contracts.Presentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Utilities.Email;
using Utilities.Gpg;
using Utilities.Logging.EventLog;

namespace Application.Batch.Infrastructure.Common;

public static class CommonServiceRegistration
{

	public static IServiceCollection AddCommonServices(this IServiceCollection services)
	{
		services.AddScoped<IApplication, Application>();
		return services;
	}
	
}