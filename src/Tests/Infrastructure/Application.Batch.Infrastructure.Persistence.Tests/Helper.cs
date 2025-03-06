using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MockQueryable.Moq;
using Moq;
using System.Threading;

namespace Application.Batch.Infrastructure.Persistence.Tests;

internal class Helper
{
	internal static Mock<IDbContext> MockContext()
	{
		{
			Mock<IDbContext> mockContext = new();
			mockContext.Setup(m => m.Customers).Returns(GetCustomers().Object);
			mockContext.Setup(m => m.Addresses).Returns(GetAddresses().Object);
			mockContext.Setup(m => m.Set<Customer>()).Returns(GetCustomers().Object);
			mockContext.Setup(m => m.Set<Address>()).Returns(GetAddresses().Object);
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
				ZipCode = "70785"
			},

			new()
			{
				Id = 2,
				CustomerId = 5,
				Street = "456 Sunset Blvd.",
				City = "Baton Rouge",
				State = "LA",
				ZipCode = "70816"
			}
		];

		return addresses.AsQueryable().BuildMockDbSet();
	}
}