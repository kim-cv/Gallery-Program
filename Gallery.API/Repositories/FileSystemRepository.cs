using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Repositories
{
    public class FileSystemRepository : IFileSystemRepository
    {
        private IWebHostEnvironment _environment;
        private IOptions<ContentFolders> _options;

        public FileSystemRepository(IWebHostEnvironment environment, IOptions<ContentFolders> options)
        {
            _environment = environment;
            _options = options;

            string UploadFolderImagesPath = ConstructUploadFolderImagesPath();
            if (Directory.Exists(UploadFolderImagesPath) == false)
            {
                Directory.CreateDirectory(UploadFolderImagesPath);
            }
        }

        public async Task<byte[]> RetrieveFile(string name, string extension)
        {
            string path = ConstructUploadFolderImagesPath();
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

        public async Task SaveFile(byte[] data, string name, string extension)
        {
            string path = ConstructUploadFolderImagesPath();
            string filename = Path.ChangeExtension(name, extension);
            string pathWithFilename = Path.Combine(path, filename);

            using (var fileStream = File.Create(pathWithFilename))
            {
                await fileStream.WriteAsync(data);
            }
        }

        public void DeleteFile(string name, string extension)
        {
            string path = ConstructUploadFolderImagesPath();
            string filename = Path.ChangeExtension(name, extension);
            string pathWithFilename = Path.Combine(path, filename);

            bool fileExist = File.Exists(pathWithFilename);

            if (fileExist == false)
            {
                return;
            }

            File.Delete(pathWithFilename);
        }

        private string ConstructUploadFolderImagesPath()
        {
            return Path.Combine(_environment.ContentRootPath, _options.Value.UploadFolderImages);
        }
    }
}
