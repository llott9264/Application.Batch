using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Utilities.UnitOfWork.Infrastructure;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public class CustomerRepository(IDbContext context) : RepositoryBase<Customer>(context), ICustomerRepository
{
	public Task<bool> IsCustomerSocialSecurityNumberUnique(string ssn, int customerId)
	{
		int matches = context.Customers.Count(c => c.SocialSecurityNumber == ssn && c.Id != customerId);
		return Task.FromResult(matches == 0);
	}

	public List<Customer> GetCustomersIncludeAddresses()
	{
		return context.Customers.Include(c => c.Addresses).ToList();
	}
}
