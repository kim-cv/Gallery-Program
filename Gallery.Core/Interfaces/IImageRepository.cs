using System.Collections.Generic;

namespace Gallery.Core.Interfaces
{
    public delegate void NewImageEventHandler(IImageInformation imageInformation);

    public interface IImageRepository
    {
        event NewImageEventHandler OnNewImage;
        IEnumerable<IImageInformation> RetrieveImages(int from, int to);
        IEnumerable<IImageInformation> RetrieveImagesUpTo(IImageInformation image);
    }
}