using Gallery.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Gallery.WPF.Interfaces;

namespace Gallery.WPF.Pages.Gallery
{
    public class GalleryViewmodel : IViewmodel
    {
        // Events & Commands
        public ICommand btnCmdChooseImage { get; set; }
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

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
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.AddGalleryLocation, null);
        }
    }
}
