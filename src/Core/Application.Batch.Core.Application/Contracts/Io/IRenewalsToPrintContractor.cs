using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface IRenewalsToPrintContractor : IOutgoingFile
{
	public string BatchName { get; }
	public bool WriteFile(List<Customer> customers);
}