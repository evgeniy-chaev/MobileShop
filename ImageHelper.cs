using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MobileShop
{
    public static class ImageHelper
    {
        public static string GetEncodedImage(Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                image.Save(m, image.RawFormat);
                byte[] imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static BitmapImage? Base64ToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            byte[] binaryData = Convert.FromBase64String(base64String);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(binaryData);
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
