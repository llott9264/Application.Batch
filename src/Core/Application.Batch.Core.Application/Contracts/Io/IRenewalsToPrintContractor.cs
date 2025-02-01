using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface IRenewalsToPrintContractor : IOutgoingFiles
{
	public string BatchName { get; }
	public bool WriteFiles(List<Customer> customers);
}