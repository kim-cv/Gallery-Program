using System.Collections.Generic;

namespace Gallery.Core.Interfaces
{
    public interface IImageFileRepository
    {
        IEnumerable<byte[]> RetrieveImages();
        byte[] RetrieveImage(string imageName);
    }
}