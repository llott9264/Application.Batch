namespace Application.Batch.Core.Domain.Common;

public class Entity
{
	public int Id { get; set; }
	public string? CreatedBy { get; set; }
	public DateTime CreatedDate { get; set; }
	public string? LastModifiedBy { get; set; }
	public DateTime? LastModifiedDate { get; set; }
}