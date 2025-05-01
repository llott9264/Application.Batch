using Application.Batch.Core.Application;
using Application.Batch.Infrastructure.Io;
using Application.Batch.Infrastructure.Pdf;
using Application.Batch.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Batch.ConsoleApp;

public static class StartupExtensions
{
	public static IConfiguration GetConfiguration(this ConfigurationBuilder builder)
	{
		builder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appSettings.json", false, true);

		return builder.Build();
	}

	public static IHost BuildHost(IConfiguration configuration)
	{
		return Host.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				services.AddSingleton<IConfiguration>(provider => configuration);
				services.AddPersistenceServices("ConnectionStrings:Customer");
				services.AddApplicationServices(configuration);
				services.AddIoServices();
				services.AddPdfServices();
			})
			.Build();
	}
}
