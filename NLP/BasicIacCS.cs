 
 
using System.Runtime.InteropServices;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using System;
using System.IO;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Properties;
using CsvHelper.TypeConversion;
using iText.Forms.Form.Element;

namespace BasicIacCS
{
    public class BasicIac  
    {
 
            public static void WriteIt(Document workingDocument,int pageNum,string textToWrite,  float X, float Y)
            {
           // var copyText = new Paragraph(textToWrite);
            workingDocument.ShowTextAligned(
            p: new Paragraph(textToWrite).SetFontSize(10f),
            x: X, y: Y,
            pageNumber: pageNum,
                textAlign: TextAlignment.LEFT, vertAlign: VerticalAlignment.TOP,
                radAngle: 0);
        }


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string source = "C:\\Users\\jhayes\\Documents\\Statements\\Edward Jones.pdf";
            string dest = "C:\\Users\\jhayes\\source\\repos\\nlp\\Edward Jones1.pdf";
            FileInfo file = new FileInfo(dest);
            file.Directory.Create();
           // PdfDocument pdfDoc =  new PdfDocument(new PdfReader(source), new PdfWriter(dest));

 

            //using var writer = new PdfWriter(dest);
            //using var outputDocument = new PdfDocument(writer);
            //using var reader = new PdfReader(source);
            //using var inputDocument = new PdfDocument(reader);
            //using var workingDocument = new Document(outputDocument);
            using var outputDocument = new PdfDocument(new PdfReader(source), new PdfWriter(dest));
            using var workingDocument = new Document(outputDocument);

 
            var numberOfPages = outputDocument.GetNumberOfPages();
            for (var i = 1; i <= numberOfPages; i++)
            {
                WriteIt(workingDocument, i, "Big effing deal", 10, 600);
                WriteIt(workingDocument, i, "Another Big effing deal", 10, 590);
            }

            //PdfPage page2 = pdfDoc.GetPage(2);
            //PdfCanvas canvas2 = new PdfCanvas(page2);
            //Rectangle pageSize2 = page2.GetPageSize();
            //float pageHeight2 = pageSize2.GetHeight();
            //canvas2.ConcatMatrix(1, 0, 0, 1, 0, 792);
            //canvas2.BeginText().SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 12)
            //    .MoveText(50, -200)     //lower left
            //    .ShowText("This better fucking be page 2")
            //    .EndText();
            workingDocument.Close();







        }

 
    }
}
