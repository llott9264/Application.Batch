using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public class CustomerRepository(IDbContext context) : Repository<Customer>(context), ICustomerRepository
{
	public Task<bool> IsCustomerSocialSecurityNumberUnique(string ssn, int customerId)
	{
		bool matches = Context.Customers.Any(c => c.SocialSecurityNumber == ssn && c.Id != customerId);
		return Task.FromResult(matches);
	}
}