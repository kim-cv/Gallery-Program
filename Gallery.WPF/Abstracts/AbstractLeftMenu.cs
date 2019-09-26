using System.Windows.Input;
using Gallery.BL;

namespace Gallery.WPF
{
    public abstract class AbstractLeftMenu : AbstractNavigation
    {
        // Commands
        public ICommand btnCmdAddGalleryLocation { get; set; }
        public ICommand btnCmdViewGalleryLocations { get; set; }

        public AbstractLeftMenu()
        {
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
    }
}
