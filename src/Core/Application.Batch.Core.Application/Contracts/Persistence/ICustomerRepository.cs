using Application.Batch.Core.Domain.Entities;
using Utilities.UnitOfWork.Contracts;

namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface ICustomerRepository : IRepositoryBase<Customer>
{
	Task<bool> IsCustomerSocialSecurityNumberUnique(string ssn, int customerId);
	List<Customer> GetCustomersIncludeAddresses();
}
