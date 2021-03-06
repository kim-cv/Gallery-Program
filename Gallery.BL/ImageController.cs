﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gallery.BL
{
    public static class ImageController
    {
        public static async Task<BitmapSource> FileInfoToThumbnail(FileInfo fileinfo)
        {
            return await Task.Run(() =>
            {
                //BitmapImage image = new BitmapImage();

                //using (MemoryStream memoryStream = new MemoryStream())
                //using (Image thumbnail = Image.FromStream(fileinfo.OpenRead()).GetThumbnailImage(100, 100, null, new IntPtr()))
                //{
                //    thumbnail.Save(memoryStream, ImageFormat.Png);

                //    image.BeginInit();
                //    image.StreamSource = memoryStream;
                //    image.CacheOption = BitmapCacheOption.OnLoad;
                //    image.EndInit();
                //    image.Freeze();
                //}

                BitmapImage bitmapImage = new BitmapImage();

                using (FileStream fileStream = fileinfo.OpenRead())
                using (Image image = Image.FromStream(fileStream))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Calculate thumb width and height with respect to aspect ratio
                    (int ThumbWidth, int ThumbHeight) = AspectRatio(image.Width, image.Height, 100, 100);

                    Image thumbnail = image.GetThumbnailImage(ThumbWidth, ThumbHeight, null, new IntPtr());
                    thumbnail.Save(memoryStream, ImageFormat.Png);

                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }

                return bitmapImage;
            });
        }

        public static async Task<BitmapSource> FileInfoToBitmapSource(FileInfo fileinfo)
        {
            return await Task.Run(() =>
            {
                BitmapImage image = new BitmapImage();
                using (FileStream memoryStream = fileinfo.OpenRead())
                {
                    image.BeginInit();
                    image.StreamSource = memoryStream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    image.Freeze();
                }
                return image;
            });
        }

        //public static async Task<BitmapSource> BytesToImage(byte[] imageBytes)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var image = new BitmapImage();
        //        using (MemoryStream memoryStream = new MemoryStream(imageBytes))
        //        {
        //            image.BeginInit();
        //            image.StreamSource = memoryStream;
        //            image.CacheOption = BitmapCacheOption.OnLoad;
        //            image.EndInit();
        //            image.Freeze();
        //        }
        //        return image;
        //    });
        //}

        //public static Task<TransformedBitmap> ConvertToThumb(BitmapSource bitmapSource, int maxWidth, int maxHeight)
        //{
        //    return Task.Run(() =>
        //    {
        //        Tuple<double, double> aspectScale = AspectRatio(bitmapSource.Width, bitmapSource.Height, maxWidth, maxHeight);

        //        var scale = new ScaleTransform(
        //            aspectScale.Item1 / bitmapSource.Width,
        //            aspectScale.Item2 / bitmapSource.Height
        //        );

        //        TransformedBitmap bitmapSourceThumb = new TransformedBitmap();
        //        bitmapSourceThumb.BeginInit();
        //        bitmapSourceThumb.Source = bitmapSource;
        //        bitmapSourceThumb.Transform = scale;
        //        bitmapSourceThumb.EndInit();
        //        bitmapSourceThumb.Freeze();

        //        return bitmapSourceThumb;
        //    });
        //}

        public static (int ThumbWidth, int ThumbHeight) AspectRatio(double currentWidth, double currentHeight, double maxWidth, double maxHeight)
        {
            // Used for aspect ratio
            double ratio = Math.Min(maxWidth / currentWidth, maxHeight / currentHeight);

            double calculatedWidth = maxWidth;
            double calculatedHeight = maxHeight;

            if (currentWidth > maxWidth)
            {
                calculatedHeight = currentHeight * ratio;
            }

            if (currentHeight > maxHeight)
            {
                calculatedWidth = currentWidth * ratio;
            }

            int finalWidth = Convert.ToInt32(calculatedWidth);
            int finalHeight = Convert.ToInt32(calculatedHeight);

            return (ThumbWidth: finalWidth, ThumbHeight: finalHeight);
        }
    }
}
