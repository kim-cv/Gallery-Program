using System.Threading.Tasks;

namespace Gallery.API.Interfaces
{
    public interface IFileSystemRepository
    {
        Task<byte[]> RetrieveFile(string path, string name, string extension);
        Task SaveFile(string path, byte[] data, string name, string extension);
        void DeleteFile(string path, string name, string extension);
    }
}
