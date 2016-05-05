using BlogSystem.BussinessLogic;
using BlogSystem.Entity;
using InfraStructure.Storage;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace CodeSnippet.Controllers
{
    public class FileSystemController : Controller
    {
        /// <summary>
        /// 缩略图（JPEG）
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Thumbnail(string filename)
        {
            var stream = new MemoryStream();
            string Thumbnail = ConfigurationManager.AppSettings["Thumbnail"];
            switch (Thumbnail)
            {
                case "Mongo":
                    MongoStorage.GetFile(stream, filename, "Thumbnail");
                    break;
                case "FileSystem":
                    stream = FileSystemStorage.GetFile(filename, "Thumbnail");
                    break;
                case "QiNue":
                    //实际上这里直接使用QiNue的URL就可以了
                    stream = QiniuStorage.GetFile(filename);
                    break;
            }
            if (stream == null) return null;
            return File(stream.ToArray(), "image/jpeg");
        }
        /// <summary>
        /// 图像文件（JPEG）
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult Image(string filename)
        {
            var stream = new MemoryStream();
            string Thumbnail = ConfigurationManager.AppSettings["Image"];
            switch (Thumbnail)
            {
                case "Mongo":
                    MongoStorage.GetFile(stream, filename, "Image");
                    break;
                case "FileSystem":
                    stream = FileSystemStorage.GetFile(filename, "Image");
                    if (stream == null) return File(new byte[] { }, "image/jpeg");
                    break;
                case "QiNue":
                    //实际上这里直接使用QiNue的URL就可以了
                    stream = QiniuStorage.GetFile(filename);
                    break;
            }
            if (stream == null) return null;
            return File(stream.ToArray(), "image/jpeg");
        }
        /// <summary>
        /// QRCode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult QRCode(string url)
        {
            var bit = QRCodeHelper.GetFile(url);
            var stream = new MemoryStream();
            bit.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            return File(stream.ToArray(), "image/bmp");
        }
        /// <summary>
        /// PDF
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [OutputCache(Duration = int.MaxValue)]
        public ActionResult PDF(string articleId)
        {
            //仅供注册用户下载
            if (Session[ConstHelper.Session_USERID] == null) return null;
            if (string.IsNullOrEmpty(articleId)) return null; ;
            var article = Article.GetArticleBySn(articleId);
            if (article == null) return null;
            if (!article.IsPrivate && article.PublishStatus != ApproveStatus.Accept)
            {
                //公开文章，但是没有发布
                return null;
            }
            var userid = Session[ConstHelper.Session_USERID].ToString();
            if (article.IsPrivate)
            {
                if (!userid.Equals(article.OwnerId)) return null;
            }
            string filepath = PDFFolder + articleId + ".pdf";
            if (!System.IO.File.Exists(filepath))
            {
                try
                {
                    ConvertUrlToPdf(BaseUrl + "/Article/SimplePdf?ArticleId=" + articleId, articleId);
                }
                catch (System.Exception ex)
                {
                    InfraStructure.Log.ExceptionLog.Log(userid, "PDF", "FileSystem", ex.ToString());
                }
            }
            PDFDownload.InsertPDFDownload(new PDFDownload() { ArticleID = articleId, UserInfoId = userid });
            return File(filepath, "application/pdf");
        }

        [NonAction]
        public static void RefreshAllPDF()
        {
            foreach (var articleId in ArticleListManager.GetListForArticleId())
            {
                string filepath = PDFFolder + articleId + ".pdf";
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                ConvertUrlToPdf(BaseUrl + "/Article/SimplePdf?ArticleId=" + articleId, articleId);
            }
        }

        /// <summary>
        /// PDF路径
        /// </summary>
        public static string PDFFolder = string.Empty;
        /// <summary>
        /// 端口号（VS本地调试和实际环境不同）
        /// </summary>
        public static string BaseUrl = string.Empty;

        /// <summary>
        /// ConvertUrlToPdf
        /// </summary>
        /// <param name="url"></param>
        /// <param name="articleid"></param>
        private static void ConvertUrlToPdf(string url, string articleid, bool WithWaterMark = false)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageSettings setting = new PdfPageSettings();
            Thread thread = new Thread(() =>
            {
                doc.LoadFromHTML(url, false, true, true, setting);
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
            doc.SaveToFile(PDFFolder + articleid + ".pdf");
            doc.Close();
        }
    }
}