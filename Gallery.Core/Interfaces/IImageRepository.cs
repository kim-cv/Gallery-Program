namespace Gallery.Core.Interfaces
{
    public delegate void NewImageEventHandler(IImageInformation imageInformation);

    public interface IImageRepository
    {
        void RetrieveImagesAsThumbs();
        IImageInformation RetrieveImage(string imageName);
        event NewImageEventHandler OnNewImage;
        IImageInformation CurrentLargeImage { get; set; }
        IImageInformation NextImage();
        IImageInformation PreviousImage();
    }
}