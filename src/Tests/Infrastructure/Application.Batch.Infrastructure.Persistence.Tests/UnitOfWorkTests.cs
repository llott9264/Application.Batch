using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests;

public class UnitOfWorkTests
{
	[Fact]
	public async Task CustomerRepository_IsCustomerSocialSecurityNumberUnique_ReturnsTrue()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);

		//Act
		bool isUnique = await unitOfWork.Customers.IsCustomerSocialSecurityNumberUnique("123456789", 6);

		//Assert
		Assert.True(isUnique);
	}

	[Fact]
	public void AddressRepository_GetAllAddresses_ReturnsTwo()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);

		//Act
		List<Address> addresses = unitOfWork.Addresses.GetAll();

		//Assert
		Assert.True(addresses.Count == 2);
	}
}
