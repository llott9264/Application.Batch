using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests.Repositories;

public class RepositoryTests
{

	[Fact]
	public void GetById_ReturnsAddress()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		CustomerRepository customerRepository = new(mockContext.Object);
		Customer? customer = customerRepository.GetById(6);

		Assert.NotNull(customer);
		Assert.True(customer.Id == 6);
	}

	[Fact]
	public async Task GetByIdAsync_ReturnsAddress()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		Address? address = await addressRepository.GetByIdAsync(6);

		Assert.NotNull(address);
		Assert.True(address.Id == 6);
	}

	[Fact]
	public void GetAll_ReturnsAllAddresses()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		List<Address> addresses = addressRepository.GetAll();

		Assert.True(addresses.Count == 2);
	}

	[Fact]
	public async Task GetAllAsync_ReturnsAllAddresses()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		AddressRepository addressRepository = new(mockContext.Object);
		List<Address> addresses = await addressRepository.GetAllAsync();

		Assert.True(addresses.Count == 2);
	}

	[Fact]
	public void Add_AddsCustomer()
	{
		Mock<IDbContext> mockContext = Helper.MockContext();
		CustomerRepository customerRepository = new(mockContext.Object);
		Customer customer = customerRepository.Add(new Customer()
		{
			FirstName = "Bob",
			LastName = "Davis",
			SocialSecurityNumber = "789456123"
		});

		mockContext.Verify(m => m.Customers.Add(It.IsAny<Customer>()), Times.Once);
	}
}