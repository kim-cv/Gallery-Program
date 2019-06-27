using System.Drawing;
using System.IO;

namespace Gallery.BL
{
    public static class ImageController
    {
        public static Image BytesToImage(byte[] imageBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(imageBytes))
            {
                return Image.FromStream(memoryStream);
            }
        }
    }
}
