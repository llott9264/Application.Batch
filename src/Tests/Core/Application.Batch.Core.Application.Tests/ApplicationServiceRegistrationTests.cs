﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Configuration.MediatR;
using Utilities.Email;
using Utilities.Email.MediatR;
using Utilities.Logging.EventLog;
using Utilities.Logging.EventLog.MediatR;

namespace Application.Batch.Core.Application.Tests;

public class ApplicationServiceRegistrationTests
{
	private readonly IConfigurationRoot _configuration = new ConfigurationBuilder()
		.AddInMemoryCollection(new Dictionary<string, string?>
		{
			{
				"NLogConfigFile",
				@"C:\Applications\Application.Batch\src\Presentation\Application.Batch.ConsoleApp\nlog.config"
			},
			{ "SmtpServer", "" },
			{ "Port", "25" },
			{ "UserName", "" },
			{ "Password", "" },
			{ "EmailFromAddress", "noReply@llott9264.com" },
			{ "Environment", "LocalDev" }
		})
		.Build();

	[Fact]
	public void AddApplicationServices_RegistersAllServices_CorrectlyResolvesTypes()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		IMediator? mediator = serviceProvider.GetService<IMediator>();
		IMapper? mapper = serviceProvider.GetService<IMapper>();
		IEmail? email = serviceProvider.GetService<IEmail>();
		ILog? log = serviceProvider.GetService<ILog>();

		// Assert
		Assert.NotNull(mediator);
		Assert.IsType<Mediator>(mediator);

		Assert.NotNull(mapper);
		Assert.IsType<AutoMapper.Mapper>(mapper);

		Assert.NotNull(email);
		Assert.IsType<Email>(email);

		Assert.NotNull(log);
		Assert.IsType<Log>(log);
	}

	[Fact]
	public void AddApplicationServices_ReturnsServiceCollection()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		IServiceCollection result = services.AddApplicationServices(_configuration);

		// Assert
		Assert.Same(services, result); // Ensures the method returns the same IServiceCollection
	}


	[Fact]
	public void AddApplicationServices_ScopedLifetime_VerifyInstanceWithinScope()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		// Assert
		using (IServiceScope scope = serviceProvider.CreateScope())
		{
			IMediator? service1 = scope.ServiceProvider.GetService<IMediator>();
			IMediator? service2 = scope.ServiceProvider.GetService<IMediator>();

			IMapper? service3 = scope.ServiceProvider.GetService<IMapper>();
			IMapper? service4 = scope.ServiceProvider.GetService<IMapper>();

			IEmail? service5 = scope.ServiceProvider.GetService<IEmail>();
			IEmail? service6 = scope.ServiceProvider.GetService<IEmail>();

			ILog? service7 = scope.ServiceProvider.GetService<ILog>();
			ILog? service8 = scope.ServiceProvider.GetService<ILog>();

			Assert.NotSame(service1, service2); //MediatR is Transient
			Assert.NotSame(service3, service4); //AutoMapper is Transient
			Assert.Same(service5, service6); //Email is Singleton
			Assert.Same(service7, service8); //Log is Singleton
		}
	}

	[Fact]
	public void AddApplicationServices_ScopedLifetime_VerifyInstancesAcrossScopes()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		// Assert
		IMediator? service1, service2;
		IMapper? service3, service4;
		IEmail? service5, service6;
		ILog? service7, service8;

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service1 = scope1.ServiceProvider.GetService<IMediator>();
		}

		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service2 = scope2.ServiceProvider.GetService<IMediator>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service3 = scope1.ServiceProvider.GetService<IMapper>();
		}

		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service4 = scope2.ServiceProvider.GetService<IMapper>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service5 = scope1.ServiceProvider.GetService<IEmail>();
		}

		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service6 = scope2.ServiceProvider.GetService<IEmail>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service7 = scope1.ServiceProvider.GetService<ILog>();
		}

		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service8 = scope2.ServiceProvider.GetService<ILog>();
		}

		Assert.NotSame(service1, service2); //MediatR is Transient
		Assert.NotSame(service3, service4); //AutoMapper is Transient
		Assert.Same(service5, service6); //Email is Singleton
		Assert.Same(service7, service8); //Log is Singleton
	}

	[Fact]
	public void AddApplicationServices_ConfigurationHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<GetConfigurationByKeyQuery, string>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}

	[Fact]
	public void AddApplicationServices_LogHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<CreateLogCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}


	[Fact]
	public void AddApplicationServices_EmailHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton<IConfiguration>(_configuration);

		// Act
		services.AddApplicationServices(_configuration);
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<SendEmailCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}
}
