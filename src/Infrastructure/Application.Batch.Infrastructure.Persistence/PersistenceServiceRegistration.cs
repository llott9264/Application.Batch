using Application.Batch.Core.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Persistence;

public static class PersistenceServiceRegistration
{
	public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer("name=ConnectionStrings:Customer"));

		services.AddScoped<IDbContext, ApplicationDbContext>();
		services.AddScoped<IUnitOfWork, UnitOfWork>();

		return services;
	}
}