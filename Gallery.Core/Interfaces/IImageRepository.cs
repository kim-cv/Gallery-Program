using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Gallery.Core.Interfaces
{
    public interface IImageRepository
    {
        IEnumerable<BitmapSource> RetrieveImagesAsThumbs();
        BitmapSource RetrieveImage(string imageName);
    }
}