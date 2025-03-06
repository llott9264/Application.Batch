using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Infrastructure.Pdf.Tests;

public class RenewalsToPrintContractorPdfTests
{
	[Fact]
	public void CreatePdf_MethodCallsCorrectly()
	{
		//Arrange
		string templatePdfPath = "PdfTemplate.pdf";
		string destinationFileFullPath = "RenewalsToPrintContractorPdf\\TestPdf.pdf";

		if (!System.IO.Directory.Exists("RenewalsToPrintContractorPdf"))
			System.IO.Directory.CreateDirectory("RenewalsToPrintContractorPdf");

		Customer[] customers = new Customer[2];
		customers[0] = new Customer()
		{
			FirstName = "John",
			LastName = "Smith",
			Addresses = new List<Address>()
			{
				new()
				{
					Street = "123 Main Street",
					City = "Baton Rouge",
					State = "LA",
					ZipCode = "70785"
				}
			}
		};

		customers[1] = new Customer()
		{
			FirstName = "Joe",
			LastName = "Jones",
			Addresses = new List<Address>()
			{
				new()
				{
					Street = "123 Main Street",
					City = "Baton Rouge",
					State = "LA",
					ZipCode = "70785"
				}
			}
		};

		RenewalsToPrintContractorPdf renewalsToPrintContractorPdf = new();

		//Act
		renewalsToPrintContractorPdf.CreatePdf(templatePdfPath, destinationFileFullPath, customers);

		//Assert
		Assert.True(File.Exists("RenewalsToPrintContractorPdf\\TestPdf.pdf"));
	}
}