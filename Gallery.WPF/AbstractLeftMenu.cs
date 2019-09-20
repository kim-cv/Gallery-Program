using System.Windows.Input;
using Gallery.BL;

namespace Gallery.WPF
{
    public abstract class AbstractLeftMenu : AbstractNavigation
    {
        // Commands
        public ICommand btnCmdChooseGallery { get; set; }
        public ICommand btnCmdAddGalleryLocation { get; set; }
        public ICommand btnCmdViewGalleryLocations { get; set; }

        public AbstractLeftMenu()
        {
            btnCmdChooseGallery = new RelayCommand<GalleryLocation>(tmpGallery =>
            {
                NavigateToChooseGallery(tmpGallery);
            });

            btnCmdAddGalleryLocation = new RelayCommand(NavigateToAddNewGalleryLocation);
            btnCmdViewGalleryLocations = new RelayCommand(NavigateToViewGalleryLocations);
        }

        private void NavigateToAddNewGalleryLocation()
        {
            NavigateToPage(AVAILABLE_PAGES.AddGalleryLocation, null);
        }

        private void NavigateToViewGalleryLocations()
        {
            NavigateToPage(AVAILABLE_PAGES.GalleryLocations, null);
        }

        private void NavigateToChooseGallery(GalleryLocation gallery)
        {
            if (gallery == null)
            {
                return;
            }

            NavigateToPage(AVAILABLE_PAGES.Gallery, gallery);
        }
    }
}
