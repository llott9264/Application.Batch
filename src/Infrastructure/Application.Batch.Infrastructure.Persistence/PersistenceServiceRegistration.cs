using Application.Batch.Core.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Persistence;

public static class PersistenceServiceRegistration
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer($"name={connectionString}"));

		services.AddScoped<IDbContext, ApplicationDbContext>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}
