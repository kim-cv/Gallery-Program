using Gallery.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Gallery.WPF.Views.Gallery
{
    public class GalleryViewmodel
    {
        public ObservableCollection<BitmapSource> Images { get; set; }
        private readonly IImageRepository imageRepositoryMediator;

        public GalleryViewmodel(IImageRepository _imageRepositoryMediator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            imageRepositoryMediator = _imageRepositoryMediator;
            IEnumerable<BitmapSource> tmpImages = imageRepositoryMediator.RetrieveImagesAsThumbs();

            Images = new ObservableCollection<BitmapSource>(tmpImages);
        }
    }
}
