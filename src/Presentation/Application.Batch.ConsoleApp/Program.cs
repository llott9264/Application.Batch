using Application.Batch.ConsoleApp;
using Application.Batch.Core.Application.Contracts.Presentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


IConfiguration configuration = new ConfigurationBuilder().GetConfiguration();
IHost host = StartupExtensions.BuildHost(configuration);

string workFlowName = args.Length > 0 ? args[0] : "default";
IApplication? service = host.Services.GetService<IApplication>();
service?.Run(workFlowName);