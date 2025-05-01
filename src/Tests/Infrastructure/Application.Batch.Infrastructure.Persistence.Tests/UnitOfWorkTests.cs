using Application.Batch.Core.Application.Contracts.Persistence;
using Application.Batch.Core.Domain.Entities;
using Moq;
using Utilities.UnitOfWork.Contracts;

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

	[Fact]
	public void Complete_WithCommandTimeOut_ReturnsTwo()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		Mock<IDatabaseFacadeWrapper> databaseFacadeMock = new Helper().MockDatabaseFacade();
		mockContext.Setup(c => c.Database).Returns(databaseFacadeMock.Object);
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);
		int commandTimeoutInSeconds = 30;
		int? originalTimeout = 10;
		int expectedRecordsChanged = 2;

		//Act
		int numberOfRecordsChanged = unitOfWork.Complete(commandTimeoutInSeconds);

		//Assert
		databaseFacadeMock.Verify(d => d.GetCommandTimeout(), Times.Once());
		databaseFacadeMock.Verify(d => d.SetCommandTimeout(commandTimeoutInSeconds), Times.Once());
		databaseFacadeMock.Verify(d => d.SetCommandTimeout(originalTimeout), Times.Once());
		mockContext.Verify(c => c.SaveChanges(), Times.Once());
		Assert.Equal(expectedRecordsChanged, numberOfRecordsChanged);
	}

	[Fact]
	public async Task CompleteAsync_WithCommandTimeOut_ReturnsTwo()
	{
		//Arrange
		Mock<IDbContext> mockContext = new Helper().MockContext();
		Mock<IDatabaseFacadeWrapper> databaseFacadeMock = new Helper().MockDatabaseFacade();
		mockContext.Setup(c => c.Database).Returns(databaseFacadeMock.Object);
		IUnitOfWork unitOfWork = new UnitOfWork(mockContext.Object);
		int commandTimeoutInSeconds = 30;
		int? originalTimeout = 10;
		int expectedRecordsChanged = 3;

		//Act
		int numberOfRecordsChanged = await unitOfWork.CompleteAsync(commandTimeoutInSeconds);

		//Assert
		databaseFacadeMock.Verify(d => d.GetCommandTimeout(), Times.Once());
		databaseFacadeMock.Verify(d => d.SetCommandTimeout(commandTimeoutInSeconds), Times.Once());
		databaseFacadeMock.Verify(d => d.SetCommandTimeout(originalTimeout), Times.Once());
		mockContext.Verify(c => c.SaveChangesAsync(CancellationToken.None), Times.Once());
		Assert.Equal(expectedRecordsChanged, numberOfRecordsChanged);
	}
}
