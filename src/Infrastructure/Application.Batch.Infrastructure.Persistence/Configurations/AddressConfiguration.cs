using Application.Batch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Batch.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
	public void Configure(EntityTypeBuilder<Address> builder)
	{
		//entity.Property(e => e.State).IsFixedLength();
		//entity.Property(e => e.ZipCode).IsFixedLength();

		//entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
		//	.OnDelete(DeleteBehavior.ClientSetNull)
		//	.HasConstraintName("FK_Address_Customer");
	}
}