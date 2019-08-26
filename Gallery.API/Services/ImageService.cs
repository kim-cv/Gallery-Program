using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Gallery.API.Interfaces;

namespace Gallery.API.Services
{
    public class ImageService : IImageService
    {
        public byte[] GenerateThumb(byte[] imageData, int maxWidth, int maxHeight, bool keepAspectRatio)
        {
            using (Image<Rgba32> image = Image.Load(imageData))
            using (var ms = new MemoryStream())
            {
                int imgWidth = image.Width;
                int imgHeight = image.Height;

                (int ThumbWidth, int ThumbHeight) = AspectRatio(imgWidth, imgHeight, maxWidth, maxHeight, keepAspectRatio);

                image.Mutate(x => x
                     .Resize(ThumbWidth, ThumbHeight));
                image.SaveAsJpeg(ms);
                return ms.ToArray();
            }
        }

        private (int ThumbWidth, int ThumbHeight) AspectRatio(double currentWidth, double currentHeight, double maxWidth, double maxHeight, bool keepAspectRatio)
        {
            // Used for aspect ratio
            double ratio = Math.Min(maxWidth / currentWidth, maxHeight / currentHeight);

            double calculatedWidth = maxWidth;
            double calculatedHeight = maxHeight;

            if (keepAspectRatio)
            {
                if (currentWidth > maxWidth)
                {
                    calculatedHeight = currentHeight * ratio;
                }

                if (currentHeight > maxHeight)
                {
                    calculatedWidth = currentWidth * ratio;
                }
            }

            int finalWidth = Convert.ToInt32(calculatedWidth);
            int finalHeight = Convert.ToInt32(calculatedHeight);

            return (ThumbWidth: finalWidth, ThumbHeight: finalHeight);
        }
    }
}
