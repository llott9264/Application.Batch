using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application.Batch.Infrastructure.Persistence.Tests;

public class ApplicationDbContextTests
{
	private ApplicationDbContext CreateInMemoryContext()
	{
		DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new ApplicationDbContext(options);
	}

	[Fact]
	public async Task SaveChangesAsync_SetsAuditFields()
	{
		// Arrange
		await using (ApplicationDbContext context = CreateInMemoryContext())
		{
			Address address = new()
			{
				Street = "123 Main St",
				City = "Test City",
				State = "TX",
				ZipCode = "12345"
			};

			// Act
			context.Addresses.Add(address);
			await context.SaveChangesAsync();

			// Assert
			Assert.True(DateTime.Now - address.CreatedDate < TimeSpan.FromSeconds(1));
			Assert.NotNull(address.CreatedBy);
			Assert.Null(address.LastModifiedDate);
			Assert.Null(address.LastModifiedBy);

			// Modify and save again
			address.Street = "456 Elm St";
			await context.SaveChangesAsync();

			Assert.NotNull(address.LastModifiedDate);
			Assert.True(DateTime.Now - address.LastModifiedDate < TimeSpan.FromSeconds(1));
			Assert.NotNull(address.LastModifiedBy);
		}
	}

	[Fact]
	public void SaveChanges_ConfiguresRelationships()
	{
		// Arrange
		using (ApplicationDbContext context = CreateInMemoryContext())
		{
			Customer customer = new()
			{
				FirstName = "John",
				LastName = "Doe",
				SocialSecurityNumber = "123456789"
			};
			Address address = new()
			{
				Street = "123 Main St",
				City = "Test City",
				State = "TX",
				ZipCode = "12345",
				Customer = customer
			};

			// Act
			context.Addresses.Add(address);
			context.SaveChanges();

			// Assert
			Address savedAddress = context.Addresses.Include(a => a.Customer).First();
			Assert.Equal(customer.Id, savedAddress.CustomerId);
			Assert.Equal(customer, savedAddress.Customer);
			Assert.Contains(address, customer.Addresses);
		}
	}

	[Fact]
	public void OnModelCreating_CustomerWithMissingFields_SavesRequiredFieldsAsEmptyStrings()
	{
		// Arrange
		using (ApplicationDbContext context = CreateInMemoryContext())
		{
			Customer invalidCustomer = new(); // Missing required fields.

			// Act & Assert
			context.Customers.Add(invalidCustomer);
			context.SaveChanges();

			Assert.True(invalidCustomer.Id > 0);
			Assert.True(invalidCustomer.FirstName == string.Empty);
			Assert.True(invalidCustomer.LastName == string.Empty);
			Assert.True(invalidCustomer.SocialSecurityNumber == string.Empty);
		}
	}

	//[Fact]
	//public void OnModelCreating_EnforcesConstraints_MaxLength()
	//{
	//	// Arrange
	//	using (ApplicationDbContext context = CreateInMemoryContext())
	//	{
	//		Customer longCustomer = new()
	//		{
	//			FirstName = new string('A', 101), // Exceeds 100 chars
	//			LastName = "Smith",
	//			SocialSecurityNumber = "123456789"
	//		};

	//		// Act & Assert
	//		context.Customers.Add(longCustomer);
	//		DbUpdateException exception = Assert.Throws<DbUpdateException>(() => context.SaveChanges());
	//		Assert.Contains("value too long", exception.InnerException?.Message.ToLower());
	//	}
	//}
}
