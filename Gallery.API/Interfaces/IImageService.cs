namespace Gallery.API.Interfaces
{
    public interface IImageService
    {
        byte[] GenerateThumb(byte[] imageData);
    }
}
