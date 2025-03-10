using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests;

internal class Helper
{
	private readonly Mock<DbSet<Customer>> _dbSetCustomer = GetCustomers();
	private readonly Mock<DbSet<Address>> _dbSetAddress = GetAddresses();

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
			//TODO: set up Database Facade
			return mockContext;
		}
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
				Addresses = new List<Address>()
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
				Addresses = new List<Address>()
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
				Customer = new()
				{
					Id = 6,
					FirstName = "John",
					LastName = "Doe",
					SocialSecurityNumber = "123456789",
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
				Customer = new()
				{
					Id = 5,
					FirstName = "Joe",
					LastName = "Jones",
					SocialSecurityNumber = "987654321",
				}
			}
		];

		return addresses.AsQueryable().BuildMockDbSet();
	}
}