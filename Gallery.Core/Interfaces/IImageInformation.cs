using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gallery.Core.Interfaces
{
    public interface IImageInformation
    {
        FileInfo fileInfo { get; set; }
        string repositoryUid { get; set; }
        BitmapSource thumb { get; set; }

        Task RetrieveThumb();
    }
}
