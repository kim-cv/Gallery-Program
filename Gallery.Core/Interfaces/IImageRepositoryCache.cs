namespace Gallery.Core.Interfaces
{
    public interface IImageRepositoryCache : IImageRepository
    {
        IImageInformation CurrentLargeImage { get; set; }
        IImageInformation NextImage();
        IImageInformation PreviousImage();
    }
}