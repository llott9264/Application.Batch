using Application.Batch.Core.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Persistence.Tests;

public class PersistenceServiceRegistrationTests
{
	[Fact]
	public void AddPersistenceServices_RegistersDbContext_WithSqlServerOptions()
	{
		// Arrange
		ServiceCollection services = new();
		IConfigurationRoot configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>
			{
				{ "ConnectionStrings:Customer", "Server=Test;Database=TestDb;Trusted_Connection=True;" }
			})
			.Build();

		// Act
		services.AddPersistenceServices(configuration);

		// Assert
		ServiceProvider provider = services.BuildServiceProvider();
		DbContextOptions<ApplicationDbContext> dbContextOptions =
			provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();

		Assert.NotNull(dbContextOptions); // Ensure options are registered
		Assert.IsType<DbContextOptions<ApplicationDbContext>>(dbContextOptions); // Correct type
		List<IDbContextOptionsExtension> extensions = dbContextOptions.Extensions.ToList();
		Assert.Contains(extensions, e => e is SqlServerOptionsExtension); // Uses SQL Server
	}

	[Fact]
	public void AddPersistenceServices_RegistersIDbContext_AsScoped()
	{
		// Arrange
		ServiceCollection services = new();
		IConfigurationRoot configuration = new ConfigurationBuilder().Build();

		// Act
		services.AddPersistenceServices(configuration);

		// Assert
		ServiceDescriptor? descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IDbContext));
		Assert.NotNull(descriptor); // Service is registered
		Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime); // Correct lifetime
		Assert.Equal(typeof(ApplicationDbContext), descriptor.ImplementationType); // Correct implementation
	}

	[Fact]
	public void AddPersistenceServices_RegistersIUnitOfWork_AsScoped()
	{
		// Arrange
		ServiceCollection services = new();
		IConfigurationRoot configuration = new ConfigurationBuilder().Build();

		// Act
		services.AddPersistenceServices(configuration);

		// Assert
		ServiceDescriptor? descriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(IUnitOfWork));
		Assert.NotNull(descriptor);
		Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
		Assert.Equal(typeof(UnitOfWork), descriptor.ImplementationType);
	}

	[Fact]
	public void AddPersistenceServices_ReturnsSameServiceCollection()
	{
		// Arrange
		ServiceCollection services = new();
		IConfigurationRoot configuration = new ConfigurationBuilder().Build();

		// Act
		IServiceCollection result = services.AddPersistenceServices(configuration);

		// Assert
		Assert.Same(services, result); // Returns same instance for chaining
	}
}
