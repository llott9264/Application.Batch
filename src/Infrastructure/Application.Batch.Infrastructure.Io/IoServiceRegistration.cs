﻿using Application.Batch.Core.Application.Contracts.Io;
using Application.Batch.Infrastructure.Io.IncomingFiles;
using Application.Batch.Infrastructure.Io.OutgoingFiles;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Batch.Infrastructure.Io;

public static class IoServiceRegistration
{
	public static IServiceCollection AddIoServices(this IServiceCollection services)
	{
		services.AddScoped<ICustomerToPrintContractor, CustomerToPrintContractor>();
		services.AddScoped<IRenewalsToPrintContractor, RenewalsToPrintContractor>();
		services.AddScoped<ICustomersFromContractor, CustomersFromContractor>();
		services.AddScoped<IRevokesFromContractor, RevokesFromContractor>();
		return services;
	}
}