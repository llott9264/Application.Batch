using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public class AddressRepository(IDbContext context) : Repository<Address>(context), IAddressRepository
{
	
}