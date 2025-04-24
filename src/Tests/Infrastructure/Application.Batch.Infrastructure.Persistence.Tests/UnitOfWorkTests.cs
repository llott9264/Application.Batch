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

	[Fact]
	public void Complete_ReturnsTwo()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);

		//Act
		int numberOfRecordsChanged = unitOfWork.Complete();

		//Assert
		Assert.True(numberOfRecordsChanged == 2);
	}

	[Fact]
	public async Task CompleteAsync_ReturnsThree()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);

		//Act
		int numberOfRecordsChanged = await unitOfWork.CompleteAsync();

		//Assert
		Assert.True(numberOfRecordsChanged == 3);
	}

	[Fact]
	public void Dispose()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);

		//Act
		unitOfWork.Dispose();

		//Assert
		mockContext.Verify(m => m.Dispose(), Times.Once());
	}
}
