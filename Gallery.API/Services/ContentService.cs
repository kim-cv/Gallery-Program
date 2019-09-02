using System.IO;
using Microsoft.AspNetCore.Hosting;
using Gallery.API.Interfaces;
using Gallery.API.Models;

namespace Gallery.API.Services
{
    public class ContentService : IContentService
    {
        private IHostingEnvironment _environment;
        private ContentFolders _folders;

        public ContentService(ContentFolders folders)
        {
            _folders = folders;
        }

        public void SetHostingEnvironment(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public string ImagesUploadFolderPath()
        {
            return Path.Combine(_environment.ContentRootPath, _folders.UploadFolderImages);
        }
    }
}
