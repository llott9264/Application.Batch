using System.ComponentModel.DataAnnotations.Schema;
using Application.Batch.Core.Domain.Common;

namespace Application.Batch.Core.Domain.Entities;

public class Address : Entity
{
	public int CustomerId { get; set; }
	public string Street { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string ZipCode { get; set; } = string.Empty;
	public virtual Customer Customer { get; set; }
}
