using System.IO;

namespace Gallery.Core.Interfaces
{
    public interface IImageFileRepository
    {
        string uid
        {
            get;
        }
        string directoryUrl
        {
            get;
        }
        //Task<IEnumerable<byte[]>> RetrieveImages();
        //byte[] RetrieveImage(string imageName);
        FileInfo[] RetrieveImages();
        FileInfo RetrieveImage(string imageName);
    }
}