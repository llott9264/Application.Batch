using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Batch.ConsoleApp;

public static class Program
{
	public static async Task Main(string[] args)
	{
		IConfiguration configuration = new ConfigurationBuilder().GetConfiguration();
		IHost host = StartupExtensions.BuildHost(configuration);
		IMediator? mediator = host.Services.GetService<IMediator>();

		ApplicationRunner runner = new(mediator);
		await runner.RunAsync(args);
	}
}