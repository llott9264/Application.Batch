using Application.Batch.Core.Domain.Entities;
using Utilities.FileManagement.Contracts;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface ICustomersToPrintContractor : IOutgoingFile
{
	public string BatchName { get; }
	public bool WriteFile(List<Customer> customers);
}
