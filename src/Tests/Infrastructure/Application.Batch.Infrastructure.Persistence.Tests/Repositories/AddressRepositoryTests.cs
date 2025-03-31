using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests.Repositories;

public class AddressRepositoryTests
{
	[Fact]
	public void GetAll_ReturnsAllAddresses()
	{
		Mock<IDbContext> mockContext = new Helper().MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		List<Address> addresses = addressRepository.GetAll();

		Assert.True(addresses.Count == 2);
	}
}