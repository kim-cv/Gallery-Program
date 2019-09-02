using Microsoft.AspNetCore.Hosting;

namespace Gallery.API.Interfaces
{
    public interface IContentService
    {
        void SetHostingEnvironment(IHostingEnvironment environment);
        string ImagesUploadFolderPath();
    }
}
