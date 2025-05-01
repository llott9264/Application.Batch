using Application.Batch.Core.Application.Contracts.Pdf;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Pdf;

public static class PdfServiceRegistration
{
	public static IServiceCollection AddPdfServices(this IServiceCollection services)
	{
		services.AddScoped<IRenewalsToPrintContractorPdf, RenewalsToPrintContractorPdf>();
		return services;
	}
}
