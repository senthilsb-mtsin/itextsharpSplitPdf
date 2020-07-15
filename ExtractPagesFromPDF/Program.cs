using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPagesFromPDF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Original PDF Path : ");
            string orgPDFPath = Console.ReadLine();
            byte[] orgPDF = File.ReadAllBytes(orgPDFPath);

            Console.WriteLine("Enter Page No to be extracted with comma separted : ");
            string pageNumbers = Console.ReadLine();

            byte[] newPDF = _extractPagesFromPDF(orgPDF, pageNumbers.Split(',').Select(x => Convert.ToInt32(x)).ToArray());

            string outPutPath = Path.Combine("Output", $"{ Path.GetFileNameWithoutExtension(orgPDFPath)}.pdf");
            File.WriteAllBytes(outPutPath, newPDF);
            Console.WriteLine("Completed");
            Console.ReadKey();
        }

        private static byte[] _extractPagesFromPDF(byte[] _oldPDFBytes, int[] pageOrder)
        {
            byte[] _pdfBytes = null;
            int rotationChange = 90;
            using (var reader = new PdfReader(_oldPDFBytes))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Document doc = new Document(reader.GetPageSizeWithRotation(1)))
                    {
                        using (PdfCopy copy = new PdfCopy(doc, ms))
                        {
                            doc.Open();
                            copy.SetLinearPageMode();
                            foreach (int pageNo in pageOrder)
                            {
                                try
                                {
                                    PdfDictionary pageDict = reader.GetPageN(pageNo);
                                    int rotation = reader.GetPageRotation(pageNo);
                                    pageDict = reader.GetPageN(pageNo);
                                    pageDict.Put(PdfName.ROTATE, new PdfNumber(rotation + rotationChange));
                                    copy.AddPage(copy.GetImportedPage(reader, pageNo));
                                }
                                catch (Exception ex)
                                {
                                    Exception e = new Exception($"PageNO : {pageNo.ToString()}", ex);
                                    throw ex;
                                }
                            }
                            doc.Close();
                        }
                    }
                    _pdfBytes = ms.ToArray();
                }
            }
            return _pdfBytes;
        }
    }
}
