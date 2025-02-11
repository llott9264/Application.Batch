using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Batch.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		//builder.HasKey(e => e.Id).HasName("PK_Customer");

		//builder.Property(e => e.CreatedBy)
		//	.HasMaxLength(100)
		//	.IsUnicode(false);
		//builder.Property(e => e.CreatedDate).HasColumnType("datetime");
		//builder.Property(e => e.FirstName)
		//	.IsRequired()
		//	.HasMaxLength(100)
		//	.IsUnicode(false);
		//builder.Property(e => e.LastModifiedBy)
		//	.HasMaxLength(100)
		//	.IsUnicode(false);
		//builder.Property(e => e.LastModifiedDate).HasColumnType("datetime");
		//builder.Property(e => e.LastName)
		//	.IsRequired()
		//	.HasMaxLength(100)
		//	.IsUnicode(false);
		//builder.Property(e => e.SocialSecurityNumber)
		//	.IsRequired()
		//	.HasMaxLength(9)
		//	.IsUnicode(false);
	}
}