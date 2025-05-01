using Application.Batch.Core.Domain.Entities;
using Application.Batch.Infrastructure.Persistence.Repositories;
using Moq;

namespace Application.Batch.Infrastructure.Persistence.Tests.Repositories
{
	public class CustomerRepositoryTests
	{
		[Fact]
		public async Task IsSsnUnique_UniqueCustomerSsn_ReturnsTrue()
		{
			Mock<IDbContext> mockContext = new Helper().MockContext();
			CustomerRepository customerRepository = new(mockContext.Object);
			bool isUnique = await customerRepository.IsCustomerSocialSecurityNumberUnique("123456789", 6);

			Assert.True(isUnique);
		}

		[Fact]
		public async Task IsSsnUnique_NonUniqueCustomerSsn_ReturnsFalse()
		{
			Mock<IDbContext> mockContext = new Helper().MockContext();
			CustomerRepository customerRepository = new(mockContext.Object);
			bool isUnique = await customerRepository.IsCustomerSocialSecurityNumberUnique("123456789", 1);

			Assert.False(isUnique);
		}

		[Fact]
		public void GetCustomers_ReturnsTrue()
		{
			Mock<IDbContext> mockContext = new Helper().MockContext();
			CustomerRepository customerRepository = new(mockContext.Object);
			List<Customer> customers = customerRepository.GetCustomersIncludeAddresses();

			Assert.True(customers.Count == 2);
			Customer? customer = customers.FirstOrDefault(c => c.Id == 6);
			Assert.NotNull(customer);
			Assert.Equal("John", customer.FirstName);
			Assert.Equal("Doe", customer.LastName);
			Assert.Equal("123456789", customer.SocialSecurityNumber);
		}
	}
}
