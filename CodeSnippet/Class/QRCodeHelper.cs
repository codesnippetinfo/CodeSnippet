using System.Drawing;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace CodeSnippet
{
    public static class QRCodeHelper
    {
        public static Bitmap GetFile(string url)
        {
            Bitmap bt;
            string enCodeString = url;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            bt = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);
            return bt;
        }
    }
}
