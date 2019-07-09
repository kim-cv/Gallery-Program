using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gallery.Core.Interfaces
{
    public interface IImageFileRepository
    {
        Task<IEnumerable<byte[]>> RetrieveImages();
        byte[] RetrieveImage(string imageName);
    }
}