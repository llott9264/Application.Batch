using Application.Batch.Core.Application.Features.Workflows.CustomersFromContractor.Commands.ProcessWorkflow;
using Utilities.FileManagement.Contracts;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface ICustomersFromContractor : IIncomingFile
{
	public string BatchName { get; }
	public Task<List<CustomerViewModel>> ReadFile();
}
