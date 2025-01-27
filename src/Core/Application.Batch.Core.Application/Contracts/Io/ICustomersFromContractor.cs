using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface ICustomersFromContractor : IIncomingFile
{
	public string BatchName { get; }
	public List<Customer> ReadFile();
}