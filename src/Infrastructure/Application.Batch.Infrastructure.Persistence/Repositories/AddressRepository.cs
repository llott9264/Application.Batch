using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using Utilities.UnitOfWork.Infrastructure;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public class AddressRepository(IDbContext context) : RepositoryBase<Address>(context), IAddressRepository
{
}
