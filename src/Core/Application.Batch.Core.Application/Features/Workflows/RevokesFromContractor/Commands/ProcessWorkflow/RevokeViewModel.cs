namespace Application.Batch.Core.Application.Features.Workflows.RevokesFromContractor.Commands.ProcessWorkflow;

public class RevokeViewModel
{
	public string SocialSecurityNumber { get; set; } = string.Empty;
	public bool IsRevoked { get; set; } = false;

}
