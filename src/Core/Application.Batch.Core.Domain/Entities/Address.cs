using Application.Batch.Core.Domain.Common;

namespace Application.Batch.Core.Domain.Entities;

public class Address : Auditable
{
	public int Id { get; set; }
	public string Street { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string ZipCode { get; set; } = string.Empty;
}