using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Utilities.UnitOfWork.Contracts;

namespace Application.Batch.Infrastructure.Persistence.Tests;

internal class Helper
{
	private readonly Mock<DbSet<Address>> _dbSetAddress = GetAddresses();
	private readonly Mock<DbSet<Customer>> _dbSetCustomer = GetCustomers();

	internal Mock<IDbContext> MockContext()
	{
		{
			Mock<IDbContext> mockContext = new();
			mockContext.Setup(m => m.Customers).Returns(_dbSetCustomer.Object);
			mockContext.Setup(m => m.Addresses).Returns(_dbSetAddress.Object);
			mockContext.Setup(m => m.Set<Customer>()).Returns(_dbSetCustomer.Object);
			mockContext.Setup(m => m.Set<Address>()).Returns(_dbSetAddress.Object);
			mockContext.Setup(m => m.SaveChanges()).Returns(2);
			mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(3));
			mockContext.Setup(m => m.Dispose());
			return mockContext;
		}
	}

	internal Mock<IDatabaseFacadeWrapper> MockDatabaseFacade()
	{
		Mock<IDatabaseFacadeWrapper> databaseFacadeMock = new();
		databaseFacadeMock.Setup(d => d.GetCommandTimeout()).Returns(10);
		databaseFacadeMock.Setup(d => d.SetCommandTimeout(30)).Verifiable();
		databaseFacadeMock.Setup(d => d.SetCommandTimeout(10)).Verifiable();
		return databaseFacadeMock;
	}

	private static Mock<DbSet<Customer>> GetCustomers()
	{
		List<Customer> customers =
		[
			new()
			{
				Id = 6,
				FirstName = "John",
				LastName = "Doe",
				SocialSecurityNumber = "123456789",
				Addresses = new List<Address>
				{
					new()
					{
						Id = 1,
						CustomerId = 6,
						Street = "123 Main Street",
						City = "Walker",
						State = "LA",
						ZipCode = "70785"
					}
				}
			},
			new()
			{
				Id = 5,
				FirstName = "Joe",
				LastName = "Jones",
				SocialSecurityNumber = "987654321",
				Addresses = new List<Address>
				{
					new()
					{
						Id = 2,
						CustomerId = 5,
						Street = "456 Sunset Blvd.",
						City = "Baton Rouge",
						State = "LA",
						ZipCode = "70816"
					}
				}
			}
		];

		return customers.AsQueryable().BuildMockDbSet();
	}

	private static Mock<DbSet<Address>> GetAddresses()
	{
		List<Address> addresses =
		[
			new()
			{
				Id = 1,
				CustomerId = 6,
				Street = "123 Main Street",
				City = "Walker",
				State = "LA",
				ZipCode = "70785",
				Customer = new Customer
				{
					Id = 6,
					FirstName = "John",
					LastName = "Doe",
					SocialSecurityNumber = "123456789"
				}
			},

			new()
			{
				Id = 2,
				CustomerId = 5,
				Street = "456 Sunset Blvd.",
				City = "Baton Rouge",
				State = "LA",
				ZipCode = "70816",
				Customer = new Customer
				{
					Id = 5,
					FirstName = "Joe",
					LastName = "Jones",
					SocialSecurityNumber = "987654321"
				}
			}
		];

		return addresses.AsQueryable().BuildMockDbSet();
	}
}
