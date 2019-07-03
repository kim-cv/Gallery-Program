using Gallery.BL;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Gallery.WPF.Views.Gallery
{
    public class GalleryViewmodel
    {
        public ObservableCollection<BitmapSource> Images { get; set; }
        private readonly FilesystemRepository filesystemRepository;

        public GalleryViewmodel(FilesystemRepository _filesystemRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            filesystemRepository = _filesystemRepository;
            IList<BitmapSource> tmpImages = filesystemRepository.RetrieveImagesAsThumbs();

            Images = new ObservableCollection<BitmapSource>(tmpImages);
        }
    }
}
