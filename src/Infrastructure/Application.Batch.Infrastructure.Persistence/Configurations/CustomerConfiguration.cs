using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Batch.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.Property(e => e.FirstName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(e => e.LastName)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(e => e.SocialSecurityNumber)
			.IsRequired()
			.HasMaxLength(9);
	}
}