using Application.Batch.Core.Domain.Common;

namespace Application.Batch.Core.Domain.Entities;

public class Customer : Entity
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string SocialSecurityNumber { get; set; } = string.Empty;
	public List<Address> Addresses { get; set; } = new();
}