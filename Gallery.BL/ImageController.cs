﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gallery.BL
{
    public static class ImageController
    {
        public static async Task<BitmapSource> BytesToImage(byte[] imageBytes)
        {
            return await Task.Run(() =>
            {
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = memoryStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            });
        }

        public static Task<TransformedBitmap> ConvertToThumb(BitmapSource bitmapSource, int maxWidth, int maxHeight)
        {
            return Task.Run(() =>
            {
                Tuple<double, double> aspectScale = AspectRatio(bitmapSource.Width, bitmapSource.Height, maxWidth, maxHeight);

                var scale = new ScaleTransform(
                    aspectScale.Item1 / bitmapSource.Width,
                    aspectScale.Item2 / bitmapSource.Height
                );

                TransformedBitmap bitmapSourceThumb = new TransformedBitmap();
                bitmapSourceThumb.BeginInit();
                bitmapSourceThumb.Source = bitmapSource;
                bitmapSourceThumb.Transform = scale;
                bitmapSourceThumb.EndInit();
                bitmapSourceThumb.Freeze();

                return bitmapSourceThumb;
            });
        }

        public static Tuple<double, double> AspectRatio(double currentWidth, double currentHeight, double maxWidth, double maxHeight)
        {
            // Used for aspect ratio
            double ratio = Math.Min(maxWidth / currentWidth, maxHeight / currentHeight);

            double finalWidth = maxWidth;
            double finalHeight = maxHeight;

            if (currentWidth > maxWidth)
            {
                finalHeight = currentHeight * ratio;
            }

            if (currentHeight > maxHeight)
            {
                finalWidth = currentWidth * ratio;
            }

            return new Tuple<double, double>(finalWidth, finalHeight);
        }
    }
}
