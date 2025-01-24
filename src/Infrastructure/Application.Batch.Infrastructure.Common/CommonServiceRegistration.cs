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

	public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
	{
		LogManager.Setup().LoadConfigurationFromFile(configuration.GetValue<string>("NLogConfigFile"));
		services.AddSingleton<ILog, Log>();
		services.AddSingleton<IEmail, Email>();
		services.AddSingleton<IGpg, Gpg>(); 
		services.AddScoped<IApplication, Application>();

		return services;
	}
	
}