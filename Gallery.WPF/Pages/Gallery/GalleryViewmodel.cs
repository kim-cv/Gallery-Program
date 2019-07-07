using Gallery.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Forms;

namespace Gallery.WPF.Views.Gallery
{
    public class GalleryViewmodel
    {
        // Events & Commands
        public ICommand btnCmdChooseImage { get; set; }

        public ObservableCollection<BitmapSource> Images { get; set; }
        private readonly IImageRepository imageRepositoryMediator;

        public GalleryViewmodel(IImageRepository _imageRepositoryMediator)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            btnCmdChooseImage = new RelayCommand<BitmapSource>(tmpImage =>
            {
                cmdChooseImage(tmpImage);
            });

            imageRepositoryMediator = _imageRepositoryMediator;
            IEnumerable<BitmapSource> tmpImages = imageRepositoryMediator.RetrieveImagesAsThumbs();

            Images = new ObservableCollection<BitmapSource>(tmpImages);
        }

        private void cmdChooseImage(BitmapSource image)
        {
            MessageBox.Show("Clicked Image");
        }
    }
}
