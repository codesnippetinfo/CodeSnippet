using Spire.Pdf;
using System;
using System.Threading;
using Spire.Pdf.HtmlConverter;
using Spire.Pdf.Graphics;
using System.Drawing;

namespace BlogDataSimulator
{
    public static class Misc
    {
        /// <summary>
        /// 备份
        /// </summary>
        /// <param name="DatabaseName"></param>
        public static void BackFile(string DatabaseName, string BackUpFolder)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public static void ConvertUrlToPdf(string url, string articleid, bool WithWaterMark = false)
        {
            PdfDocument doc = new PdfDocument();
            Thread thread = new Thread(() =>
            {
                doc.LoadFromHTML(url, false, true, true);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (WithWaterMark)
            {
                foreach (PdfPageBase page in doc.Pages)
                {
                    PdfTilingBrush brush = new PdfTilingBrush(new SizeF(page.Canvas.ClientSize.Width / 2, page.Canvas.ClientSize.Height / 3));
                    brush.Graphics.SetTransparency(0.15f);
                    brush.Graphics.Save();
                    brush.Graphics.TranslateTransform(brush.Size.Width / 2, brush.Size.Height / 2);
                    brush.Graphics.RotateTransform(-45);
                    brush.Graphics.DrawString("www.codesnippet.info",
                        new PdfFont(PdfFontFamily.Helvetica, 24), PdfBrushes.Violet, 0, 0,
                        new PdfStringFormat(PdfTextAlignment.Center));
                    brush.Graphics.Restore();
                    brush.Graphics.SetTransparency(1);
                    page.Canvas.DrawRectangle(brush, new RectangleF(new PointF(0, 0), page.Canvas.ClientSize));
                }
            }
            //Save pdf file.
            doc.SaveToFile(articleid + ".pdf");
            doc.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        public static void ConvertHtmlToPdf(string html)
        {
            //Create a pdf document.
            PdfDocument doc = new PdfDocument();
            PdfPageSettings page = new PdfPageSettings();
            PdfHtmlLayoutFormat format = new PdfHtmlLayoutFormat();
            doc.LoadFromHTML(html, true, page, format);
            //Save pdf file.
            doc.SaveToFile("sample.pdf");
            doc.Close();
            //Launching the Pdf file.
            System.Diagnostics.Process.Start("sample.pdf");

        }
    }
}
