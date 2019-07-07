using Gallery.BL;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public class GalleryLocationsViewmodel
    {
        // Events & Commands
        public ICommand btnCmdChooseGallery { get; set; }
        public ICommand btnCmdAddGalleryLocation { get; set; }
        public delegate void NavigateToPageEventHandler(AVAILABLE_PAGES page);
        public event NavigateToPageEventHandler OnNavigateToNewPage;

        public ObservableCollection<GalleryLocation> GalleryLocations
        { get; set; }
        private readonly GalleryDataSQLiteRepository galleryDataSQLiteRepository;

        public GalleryLocationsViewmodel(GalleryDataSQLiteRepository _galleryDataSQLiteRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            btnCmdChooseGallery = new RelayCommand<GalleryLocation>(tmpGallery =>
            {
                cmdChooseGallery(tmpGallery);
            });

            galleryDataSQLiteRepository = _galleryDataSQLiteRepository;
            GalleryLocations = new ObservableCollection<GalleryLocation>(galleryDataSQLiteRepository.RetrieveGalleryLocations());

            btnCmdAddGalleryLocation = new RelayCommand(NavigateToAddNewGalleryLocation);
        }

        private void NavigateToAddNewGalleryLocation()
        {
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.AddGalleryLocation);
        }

        private void cmdChooseGallery(GalleryLocation gallery)
        {
        }
    }
}
