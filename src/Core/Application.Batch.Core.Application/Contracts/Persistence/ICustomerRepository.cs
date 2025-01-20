using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
	Task<bool> IsCustomerSocialSecurityNumberUnique(string ssn, int customerId);
}