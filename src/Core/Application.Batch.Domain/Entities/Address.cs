using Application.Batch.Domain.Common;

namespace Application.Batch.Domain.Entities;

public class Address : AuditableEntity
{
	public int Id { get; set; }
	public string Street { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string ZipCode { get; set; } = string.Empty;
}