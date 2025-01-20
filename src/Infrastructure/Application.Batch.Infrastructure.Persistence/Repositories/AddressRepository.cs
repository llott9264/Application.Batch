using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Infrastructure.Persistence.Repositories;

public class AddressRepository(ApplicationDbContext context) : Repository<Address>(context), IAddressRepository
{
	
}