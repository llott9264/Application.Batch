using Application.Batch.Core.Application.Contracts.Pdf;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;

namespace Application.Batch.Infrastructure.Pdf
{
	public class Pdf : IPdf
	{
		public void CreatePdf(string templatePath, string destinationPath)
		{
			PdfDocument pdf = new(new PdfWriter(destinationPath));
			PdfDocument origPdf = new(new PdfReader(templatePath));
			PdfPage origPage = origPdf.GetPage(1);
			pdf.AddPage(origPage.CopyTo(pdf));
			PdfPage newPage = pdf.GetPage(1);

			PdfCanvas canvas = new(newPage);
			Rectangle? pageSize = newPage.GetPageSize();
			canvas.BeginText()
				.SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN), 12)
				.MoveText(pageSize.GetWidth() / 2 - 24, pageSize.GetHeight() - 10)
				.ShowText("Hello World")
				.EndText();

			pdf.Close();
			origPdf.Close();
		}
	}
}
