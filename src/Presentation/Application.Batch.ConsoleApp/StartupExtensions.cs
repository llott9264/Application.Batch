using Application.Batch.Core.Application;
using Application.Batch.Infrastructure.Io;
using Application.Batch.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Application.Batch.ConsoleApp;

public static class StartupExtensions
{
	public static IConfigurationBuilder BuildConfiguration(this IConfigurationBuilder builder)
	{
		builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
		return builder;
	}

	public static IConfiguration GetConfiguration(this ConfigurationBuilder builder)
	{
		return builder.BuildConfiguration().Build();
	}

	public static IHost BuildHost(IConfiguration configuration)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				services.AddPersistenceServices(configuration);
				services.AddApplicationServices(configuration);
				services.AddIoServices();
			})
			.Build();
	}
}