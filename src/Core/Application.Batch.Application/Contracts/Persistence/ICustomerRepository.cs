using Application.Batch.Domain.Entities;

namespace Application.Batch.Application.Contracts.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
	Task<bool> IsCustomerSocialSecurityNumberUnique(string ssn);
}