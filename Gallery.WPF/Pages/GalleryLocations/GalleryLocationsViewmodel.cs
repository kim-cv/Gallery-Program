using Gallery.BL;
using Gallery.WPF.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public class GalleryLocationsViewmodel : IViewmodel
    {
        // Events & Commands
        public ICommand btnCmdChooseGallery { get; set; }
        public ICommand btnCmdAddGalleryLocation { get; set; }
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

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
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.AddGalleryLocation, null);
        }

        private void cmdChooseGallery(GalleryLocation gallery)
        {
            if (gallery == null)
            {
                return;
            }

            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.Gallery, gallery);
        }
    }
}
