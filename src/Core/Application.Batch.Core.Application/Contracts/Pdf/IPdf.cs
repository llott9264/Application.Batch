namespace Application.Batch.Core.Application.Contracts.Pdf;

public interface IPdf
{
	void CreatePdf(string templatePath, string destinationPath);
}