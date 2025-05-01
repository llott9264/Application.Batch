using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Core.Application.Contracts.Pdf;

public interface IRenewalsToPrintContractorPdf
{
	void CreatePdf(string templatePath, string destinationPath, Customer[] customers);
}
