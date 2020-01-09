using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Gallery.Core.Interfaces;

namespace Gallery.BL
{
    public class ImageInformation : IImageInformation, INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        public FileInfo fileInfo { get; set; }
        public string repositoryUid { get; set; }
        public BitmapSource thumb { get; set; }
        public BitmapSource fullImage { get; set; }

        public ImageInformation(string _repositoryUid, FileInfo _fileInfo)
        {
            repositoryUid = _repositoryUid;
            fileInfo = _fileInfo;
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public async Task RetrieveThumb()
        {
            if (thumb == null)
            {
                thumb = await ImageController.FileInfoToThumbnail(fileInfo);
                NotifyPropertyChanged("thumb");
            }
        }

        public async Task RetrieveFullImage()
        {
            if (fullImage == null)
            {
                fullImage = await ImageController.FileInfoToBitmapSource(fileInfo);
                NotifyPropertyChanged("fullImage");
            }
        }
    }
}
