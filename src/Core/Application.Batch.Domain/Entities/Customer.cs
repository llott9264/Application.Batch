using Application.Batch.Domain.Common;

namespace Application.Batch.Domain.Entities;

public class Customer : AuditableEntity
{
	public int Id { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string SocialSecurityNumber { get; set; } = string.Empty;
	public List<Address> Addresses { get; set; } = new();
}