using Application.Batch.Core.Application.Contracts.Pdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using Application.Batch.Core.Domain.Entities;

namespace Application.Batch.Infrastructure.Pdf
{
	public class RenewalsToPrintContractorPdf : IRenewalsToPrintContractorPdf
	{
		public void CreatePdf(string templateFileFullPath, string destinationFileFullPath, Customer[] customers)
		{
			using (PdfDocument pdf = new(new PdfWriter(destinationFileFullPath)))
			{
				using (PdfDocument templatePdf = new(new PdfReader(templateFileFullPath)))
				{
					PdfPage templatePage = templatePdf.GetPage(1);
					int counter = 1;

					foreach (Customer customer in customers)
					{
						pdf.AddPage(templatePage.CopyTo(pdf));
						PdfPage newPage = pdf.GetLastPage();
						PdfCanvas pdfCanvas = new(newPage);

						pdfCanvas.BeginText()
							.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
							.MoveText(182, 555)
							.ShowText($"{customer.FirstName} {customer.LastName}")
							.EndText();

						Address? address = customer.Addresses.FirstOrDefault();

						if (address != null)
						{
							const int addressAreaStartingHeight = 650;

							pdfCanvas.BeginText()
								.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
								.MoveText(155, addressAreaStartingHeight)
								.ShowText($"{customer.FirstName} {customer.LastName}")
								.EndText();

							pdfCanvas.BeginText()
								.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
								.MoveText(155, addressAreaStartingHeight - 15)
								.ShowText(address.Street)
								.EndText();

							pdfCanvas.BeginText()
								.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
								.MoveText(155, addressAreaStartingHeight - 30)
								.ShowText($"{address.City}, {address.State} {address.ZipCode}")
								.EndText();
						}

						pdfCanvas.BeginText()
							.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
							.MoveText(100, 100)
							.ShowText($"{counter}")
							.EndText();

						counter++;
					}
				}
			}
		}
	}
}
