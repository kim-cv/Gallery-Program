using System.Windows.Media.Imaging;

namespace Gallery.Core.Interfaces
{
    public delegate void NewImageEventHandler(BitmapSource bitmapSource);

    public interface IImageRepository
    {
        void RetrieveImagesAsThumbs();
        BitmapSource RetrieveImage(string imageName);
        event NewImageEventHandler OnNewImage;
    }
}