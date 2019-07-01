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

        public GalleryViewmodel()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            FilesystemRepository filesystemRepository = new FilesystemRepository();
            IList<BitmapSource> tmpImages = filesystemRepository.RetrieveImagesAsThumbs();

            Images = new ObservableCollection<BitmapSource>(tmpImages);
        }
    }
}
