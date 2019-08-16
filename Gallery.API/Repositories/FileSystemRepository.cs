using System.IO;
using System.Threading.Tasks;
using Gallery.API.Interfaces;

namespace Gallery.API.Repositories
{
    public class FileSystemRepository : IFileSystemRepository
    {
        public async Task<byte[]> RetrieveFile(string path, string name, string extension)
        {
            string filename = Path.ChangeExtension(name, extension);
            string pathWithFilename = Path.Combine(path, filename);

            byte[] data;
            using (var fileStream = File.OpenRead(pathWithFilename))
            {
                data = new byte[fileStream.Length];
                await fileStream.ReadAsync(data, 0, (int)fileStream.Length);
            }

            return data;
        }

        public async Task SaveFile(string path, byte[] data, string name, string extension)
        {
            string filename = Path.ChangeExtension(name, extension);
            string pathWithFilename = Path.Combine(path, filename);

            using (var fileStream = File.Create(pathWithFilename))
            {
                await fileStream.WriteAsync(data);
            }
        }
    }
}
