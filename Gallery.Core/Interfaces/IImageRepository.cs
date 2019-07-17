using System.Collections.Generic;

namespace Gallery.Core.Interfaces
{
    public delegate void NewImageEventHandler(IImageInformation imageInformation);

    public interface IImageRepository
    {
        IImageInformation RetrieveImage(string imageName);
        event NewImageEventHandler OnNewImage;
        IImageInformation CurrentLargeImage { get; set; }
        IImageInformation NextImage();
        IImageInformation PreviousImage();
        IEnumerable<IImageInformation> RetrieveImages(int from, int to);
    }
}