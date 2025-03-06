using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests.Repositories;

public class RepositoryTests
{
	[Fact]
	public void GetAll_ReturnsAllAddresses()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		List<Address> addresses = addressRepository.GetAll();

		Assert.True(addresses.Count == 2);
	}

	[Fact]
	public void GetById_ReturnsAddresses()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		bool doesExist = addressRepository.DoesExist(a => a.Id == 2);

		Assert.True(doesExist);
	}
}