using Utilities.UnitOfWork.Contracts;

namespace Application.Batch.Core.Application.Contracts.Persistence;

public interface IUnitOfWork : IUnitOfWorkBase
{
	ICustomerRepository Customers { get; }
	IAddressRepository Addresses { get; }
}
