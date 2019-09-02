using System.Threading.Tasks;

namespace Gallery.API.Interfaces
{
    public interface IFileSystemRepository
    {
        Task<byte[]> RetrieveFile(string name, string extension);
        Task SaveFile(byte[] data, string name, string extension);
        void DeleteFile(string name, string extension);
    }
}
