using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace Application.Batch.Infrastructure.Persistence.Tests.Repositories;

public class RepositoryTests
{
	private readonly Mock<IDbContext> _dbContextMock = new Helper().MockContext();
	private readonly Mock<DbSet<Address>> _dbSetMock;
	private readonly AddressRepository _repository;

	public RepositoryTests()
	{
		//Arrange
		_repository = new AddressRepository(_dbContextMock.Object);
		_dbSetMock = Mock.Get(_dbContextMock.Object.Addresses);
	}

	[Fact]
	public void GetById_ReturnsEntity()
	{
		// Act
		_repository.GetById(1);

		// Assert
		_dbSetMock.Verify(m => m.Find(1), Times.Once);
	}

	[Fact]
	public async Task GetByIdAsync_ReturnsEntity()
	{
		//Act
		await _repository.GetByIdAsync(6);

		//Assert
		_dbSetMock.Verify(m => m.FindAsync(6), Times.Once);
	}

	[Fact]
	public void GetAll_ReturnsAllEntities()
	{
		//Act
		List<Address> addresses = _repository.GetAll();

		//Assert
		Assert.True(addresses.Count == 2);
	}

	[Fact]
	public async Task GetAllAsync_ReturnsAllEntities()
	{
		List<Address> addresses = await _repository.GetAllAsync();

		Assert.True(addresses.Count == 2);
	}

	[Fact]
	public void Add_AddsEntity()
	{
		// Arrange
		Address address = new() { Id = 3, Street = "789 Elm St" };

		// Act
		Address result = _repository.Add(address);

		// Assert

		_dbSetMock.Verify(m => m.Add(address), Times.Once);
		Assert.Equal(address, result);
	}

	[Fact]
	public void AddRange_AddsEntities()
	{
		// Arrange
		List<Address> addresses = [new() { Id = 3 }, new() { Id = 4 }];

		// Act
		_repository.AddRange(addresses);

		// Assert
		_dbSetMock.Verify(m => m.AddRange(addresses), Times.Once);
	}

	[Fact]
	public void Update_UpdatesEntity()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "Updated St" };

		// Act
		_repository.Update(address);

		// Assert
		_dbSetMock.Verify(m => m.Update(address), Times.Once);
	}

	[Fact]
	public void Remove_RemovesEntity()
	{
		// Arrange
		Address address = new() { Id = 1 };

		// Act
		_repository.Remove(address);

		// Assert
		_dbSetMock.Verify(m => m.Remove(address), Times.Once);
	}

	[Fact]
	public void RemoveRange_RemovesEntities()
	{
		// Arrange
		List<Address> addresses = [new() { Id = 1 }, new() { Id = 2 }];

		// Act
		_repository.RemoveRange(addresses);

		// Assert
		_dbSetMock.Verify(m => m.RemoveRange(addresses), Times.Once);
	}

	[Fact]
	public void RemoveAll_RemovesAllEntities()
	{
		// Act
		_repository.RemoveAll();

		// Assert
		_dbSetMock.Verify(m => m.RemoveRange(It.Is<List<Address>>(l => l.Count == 2)), Times.Once);
	}

	[Fact]
	public void Find_ReturnsMatchingEntities()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Street == "123 Main Street";

		// Act
		List<Address> result = _repository.Find(predicate);

		// Assert
		Assert.Single(result);
		Assert.Equal("123 Main Street", result[0].Street);
	}

	[Fact]
	public async Task FindAsync_ReturnsMatchingEntities()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Street == "456 Sunset Blvd.";

		// Act
		List<Address> result = await _repository.FindAsync(predicate);

		// Assert
		Assert.Single(result);
		Assert.Equal("456 Sunset Blvd.", result[0].Street);
	}

	[Fact]
	public void Find_WithoutIncludes_ReturnsMatchingEntity()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "123 Main Street" };
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;

		// Act
		Address? result = _repository.Find(predicate, null);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);
	}

	[Fact]
	public void Find_WithIncludes_ReturnsFirstMatch()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "123 Main Street" };
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;
		List<string> includes = ["Customer"];

		// Act
		Address? result = _repository.Find(predicate, includes);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);

		Assert.True(result.Customer.Id == 6);
		Assert.True(result.Customer.FirstName == "John");
	}

	[Fact]
	public void Find_NoMatch_ReturnsNull()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Id == 999;

		// Act
		Address? result = _repository.Find(predicate, null);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.Null(result);
	}

	[Fact]
	public void Find_EmptyIncludes_DoesNotCallInclude()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "123 Main Street"};
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;
		List<string> includes = new();

		// Act
		Address? result = _repository.Find(predicate, includes);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);
	}

	[Fact]
	public async Task FindAsync_WithoutIncludes_ReturnsMatchingEntity()
	{
		// Arrange
		var address = new Address { Id = 1, Street = "123 Main Street" };
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;

		//IQueryable<Address> data = new List<Address> { address }.AsQueryable();
		//_dbSetMock.Setup(m => m.FirstOrDefault(predicate)).Returns(address);

		// Act
		Address? result = await _repository.FindAsync(predicate, null);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);
	}

	[Fact]
	public async Task FindAsync_WithIncludes_ReturnsFirstMatch()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "123 Main Street" };
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;
		List<string> includes = ["Customer"];

		// Act
		Address? result = await _repository.FindAsync(predicate, includes);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);

		Assert.True(result.Customer.Id == 6);
		Assert.True(result.Customer.FirstName == "John");
	}

	[Fact]
	public async Task FindAsync_NoMatch_ReturnsNull()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Id == 999;

		// Act
		Address? result = await _repository.FindAsync(predicate, null);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.Null(result);
	}

	[Fact]
	public async Task FindAsync_EmptyIncludes_DoesNotCallInclude()
	{
		// Arrange
		Address address = new() { Id = 1, Street = "123 Main Street" };
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;
		List<string> includes = new();

		// Act
		Address? result = await _repository.FindAsync(predicate, includes);

		// Assert
		_dbContextMock.Verify(m => m.Set<Address>(), Times.Once);
		Assert.NotNull(result);
		Assert.Equal(address.Id, result.Id);
		Assert.Equal(address.Street, result.Street);
	}

	[Fact]
	public void DoesExist_ReturnsTrueIfMatch()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Id == 1;

		// Act
		bool result = _repository.DoesExist(predicate);

		// Assert
		Assert.True(result); // Pre-configured data has Id 1
	}

	[Fact]
	public async Task DoesExistAsync_ReturnsTrueIfMatch()
	{
		// Arrange
		Expression<Func<Address, bool>> predicate = a => a.Id == 2;

		// Act
		bool result = await _repository.DoesExistAsync(predicate);

		// Assert
		Assert.True(result); // Pre-configured data has Id 2
	}
}