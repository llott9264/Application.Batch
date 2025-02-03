using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Utilities.Email;
using Utilities.Gpg;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

		LogManager.Setup().LoadConfigurationFromFile(configuration.GetValue<string>("NLogConfigFile"));
		services.AddSingleton<ILog, Log>();
		services.AddSingleton<IEmail, Email>();
		services.AddSingleton<IGpg, Gpg>();

		return services;
	}
}