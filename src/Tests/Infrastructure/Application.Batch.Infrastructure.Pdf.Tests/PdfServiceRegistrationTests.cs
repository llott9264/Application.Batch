using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Core.Application.Contracts.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Pdf.Tests
{
	public class PdfServiceRegistrationTests
	{
		[Fact]
		public void AddPdfServices_RegistersAllServices_CorrectlyResolvesTypes()
		{
			//Arrange
			ServiceCollection services = new();

			//Act
			services.AddPdfServices();
			ServiceProvider serviceProvider = services.BuildServiceProvider();
			IRenewalsToPrintContractorPdf? renewalsToPrintContractorPdf = serviceProvider.GetService<IRenewalsToPrintContractorPdf>();

			//Assert
			Assert.NotNull(renewalsToPrintContractorPdf);
			Assert.IsType<RenewalsToPrintContractorPdf>(renewalsToPrintContractorPdf);
		}

		[Fact]
		public void AddPdfServices_ReturnsServiceCollection()
		{
			// Arrange
			ServiceCollection services = new();

			// Act
			IServiceCollection result = services.AddPdfServices();

			// Assert
			Assert.Same(services, result); // Ensures the method returns the same IServiceCollection
		}

		[Fact]
		public void AddPdfServices_ScopedLifetime_SameInstanceWithinScope()
		{
			// Arrange
			ServiceCollection services = new();

			// Act
			services.AddPdfServices();
			ServiceProvider serviceProvider = services.BuildServiceProvider();

			// Assert
			using (IServiceScope scope = serviceProvider.CreateScope())
			{
				IRenewalsToPrintContractorPdf? service1 = scope.ServiceProvider.GetService<IRenewalsToPrintContractorPdf>();
				IRenewalsToPrintContractorPdf? service2 = scope.ServiceProvider.GetService<IRenewalsToPrintContractorPdf>();

				// Verify same instance within the same scope
				Assert.Same(service1, service2);
			}
		}

		[Fact]
		public void AddIoServices_ScopedLifetime_DifferentInstancesAcrossScopes()
		{
			// Arrange
			ServiceCollection services = new();

			// Act
			services.AddPdfServices();
			ServiceProvider serviceProvider = services.BuildServiceProvider();

			// Assert
			IRenewalsToPrintContractorPdf? service1, service2;

			using (IServiceScope scope1 = serviceProvider.CreateScope())
			{
				service1 = scope1.ServiceProvider.GetService<IRenewalsToPrintContractorPdf>();
			}
			using (IServiceScope scope2 = serviceProvider.CreateScope())
			{
				service2 = scope2.ServiceProvider.GetService<IRenewalsToPrintContractorPdf>();
			}

			// Different instances across scopes
			Assert.NotSame(service1, service2);
		}
	}
}
