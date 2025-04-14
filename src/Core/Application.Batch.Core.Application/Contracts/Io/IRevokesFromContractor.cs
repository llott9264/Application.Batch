using Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;
using Utilities.FileManagement.Contracts;

namespace Application.Batch.Core.Application.Contracts.Io;

public interface IRevokesFromContractor : IIncomingFiles
{
	public string BatchName { get; }
	public List<RevokeViewModel> ReadFiles();
}
