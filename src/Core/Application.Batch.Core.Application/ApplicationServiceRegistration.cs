﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Configuration;
using Utilities.Email;
using Utilities.Logging.EventLog;

namespace Application.Batch.Core.Application;

public static class ApplicationServiceRegistration
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
		services.AddConfigurationServices();
		services.AddLoggerServices(configuration);
		services.AddEmailServices();
		return services;
	}
}
