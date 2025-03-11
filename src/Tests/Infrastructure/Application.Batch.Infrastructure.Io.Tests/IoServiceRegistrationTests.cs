using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Pdf;
using Application.Batch.Core.Application.Features.Mapper;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using Application.Batch.Infrastructure.Pdf;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Utilities.Configuration.MediatR;

namespace Application.Batch.Infrastructure.Io.Tests;

public class IoServiceRegistrationTests
{
	private const string ArchiveFolderBasePath = "MyArchiveFolderPath\\";
	private const string DataTransferFolderBasePath = "MyDataTransferFolderPath\\";
	private const string GpgPrivateKeyName = "MyPublicKey.asc";
	private const string GpgPrivateKeyPassword = "password";

	private static Mock<IMediator> GetMockMediator()
	{
		Mock<IMediator> mock = new();
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:ArchivePath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(ArchiveFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:DataTransferPath"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(DataTransferFolderBasePath));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyName"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyName));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "Workflows:RevokesFromContractor:PrivateKeyPassword"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GpgPrivateKeyPassword));
		mock.Setup(m => m.Send(It.Is<GetConfigurationByKeyQuery>(request => request.Key == "FileRetentionPeriodInMonths"), It.IsAny<CancellationToken>())).Returns(Task.FromResult("-13"));
		return mock;
	}

	private static IMapper GetMapper()
	{
		MapperConfiguration configuration = new(cfg => cfg.AddProfile<MappingProfile>());
		Mapper mapper = new(configuration);
		return mapper;
	}

	[Fact]
	public void AddIoServices_RegistersAllServices_CorrectlyResolvesTypes()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton(GetMockMediator().Object);
		services.AddSingleton(GetMapper());
		services.AddScoped<IRenewalsToPrintContractorPdf, RenewalsToPrintContractorPdf>();

		// Act
		services.AddIoServices();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		ICustomerToPrintContractor? customerToPrint = serviceProvider.GetService<ICustomerToPrintContractor>();
		IRenewalsToPrintContractor? renewalsToPrint = serviceProvider.GetService<IRenewalsToPrintContractor>();
		ICustomersFromContractor? customersFrom = serviceProvider.GetService<ICustomersFromContractor>();
		IRevokesFromContractor? revokesFrom = serviceProvider.GetService<IRevokesFromContractor>();

		// Assert
		Assert.NotNull(customerToPrint);
		Assert.IsType<CustomersToPrintContractor>(customerToPrint);

		Assert.NotNull(renewalsToPrint);
		Assert.IsType<RenewalsToPrintContractor>(renewalsToPrint);

		Assert.NotNull(customersFrom);
		Assert.IsType<CustomersFromContractor>(customersFrom);

		Assert.NotNull(revokesFrom);
		Assert.IsType<RevokesFromContractor>(revokesFrom);
	}

	[Fact]
	public void AddIoServices_ReturnsServiceCollection()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton(GetMockMediator().Object);
		services.AddSingleton(GetMapper());
		services.AddScoped<IRenewalsToPrintContractorPdf, RenewalsToPrintContractorPdf>();

		// Act
		IServiceCollection result = services.AddIoServices();

		// Assert
		Assert.Same(services, result); // Ensures the method returns the same IServiceCollection
	}

	[Fact]
	public void AddIoServices_ScopedLifetime_SameInstanceWithinScope()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton(GetMockMediator().Object);
		services.AddSingleton(GetMapper());
		services.AddScoped<IRenewalsToPrintContractorPdf, RenewalsToPrintContractorPdf>();

		// Act
		services.AddIoServices();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		// Assert
		using (IServiceScope scope = serviceProvider.CreateScope())
		{
			ICustomerToPrintContractor? service1 = scope.ServiceProvider.GetService<ICustomerToPrintContractor>();
			ICustomerToPrintContractor? service2 = scope.ServiceProvider.GetService<ICustomerToPrintContractor>();

			IRenewalsToPrintContractor? service3 = scope.ServiceProvider.GetService<IRenewalsToPrintContractor>();
			IRenewalsToPrintContractor? service4 = scope.ServiceProvider.GetService<IRenewalsToPrintContractor>();

			ICustomersFromContractor? service5 = scope.ServiceProvider.GetService<ICustomersFromContractor>();
			ICustomersFromContractor? service6 = scope.ServiceProvider.GetService<ICustomersFromContractor>();

			IRevokesFromContractor? service7 = scope.ServiceProvider.GetService<IRevokesFromContractor>();
			IRevokesFromContractor? service8 = scope.ServiceProvider.GetService<IRevokesFromContractor>();

			// Verify same instance within the same scope
			Assert.Same(service1, service2); 
			Assert.Same(service3, service4);
			Assert.Same(service5, service6);
			Assert.Same(service7, service8);
		}
	}

	[Fact]
	public void AddIoServices_ScopedLifetime_DifferentInstancesAcrossScopes()
	{
		// Arrange
		ServiceCollection services = new();
		services.AddSingleton(GetMockMediator().Object);
		services.AddSingleton(GetMapper());
		services.AddScoped<IRenewalsToPrintContractorPdf, RenewalsToPrintContractorPdf>();

		// Act
		services.AddIoServices();
		ServiceProvider serviceProvider = services.BuildServiceProvider();

		// Assert
		ICustomerToPrintContractor? service1, service2;
		IRenewalsToPrintContractor? service3, service4;
		ICustomersFromContractor? service5, service6;
		IRevokesFromContractor? service7, service8;

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service1 = scope1.ServiceProvider.GetService<ICustomerToPrintContractor>();
		}
		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service2 = scope2.ServiceProvider.GetService<ICustomerToPrintContractor>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service3 = scope1.ServiceProvider.GetService<IRenewalsToPrintContractor>();
		}
		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service4 = scope2.ServiceProvider.GetService<IRenewalsToPrintContractor>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service5 = scope1.ServiceProvider.GetService<ICustomersFromContractor>();
		}
		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service6 = scope2.ServiceProvider.GetService<ICustomersFromContractor>();
		}

		using (IServiceScope scope1 = serviceProvider.CreateScope())
		{
			service7 = scope1.ServiceProvider.GetService<IRevokesFromContractor>();
		}
		using (IServiceScope scope2 = serviceProvider.CreateScope())
		{
			service8 = scope2.ServiceProvider.GetService<IRevokesFromContractor>();
		}

		// Different instances across scopes
		Assert.NotSame(service1, service2);
		Assert.NotSame(service3, service4);
		Assert.NotSame(service5, service6);
		Assert.NotSame(service7, service8);
	}

	[Fact]
	public void AddIoOperationsServices_DirectoryCleanUpHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		services.AddIoOperationsServices();
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<CleanUpDirectoryCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}

	[Fact]
	public void AddIoOperationsServices_CreateDirectoryHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		services.AddIoOperationsServices();
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<CreateDirectoryCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}

	[Fact]
	public void AddIoOperationsServices_DeleteFilesHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		services.AddIoOperationsServices();
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<DeleteFilesCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}

	[Fact]
	public void AddIoOperationsServices_CopyFileHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		services.AddIoOperationsServices();
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<CopyFileCommand>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}

	[Fact]
	public void AddIoOperationsServices_MoveFileHandler_VerifyMediatorHandlerExists()
	{
		// Arrange
		ServiceCollection services = new();

		// Act
		services.AddIoOperationsServices();
		List<ServiceDescriptor> serviceDescriptors = services.ToList();

		// Assert
		ServiceDescriptor? handlerDescriptor = serviceDescriptors.FirstOrDefault(sd =>
			sd.ServiceType == typeof(IRequestHandler<MoveFileCommand, bool>));

		Assert.NotNull(handlerDescriptor);
		Assert.Equal(ServiceLifetime.Transient, handlerDescriptor.Lifetime);
	}
}
