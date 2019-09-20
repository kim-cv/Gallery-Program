using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Gallery.BL;
using Gallery.WPF.Interfaces;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public class GalleryLocationsViewmodel : AbstractLeftMenu, IViewmodel
    {
        // Commands
        public ICommand btnCmdChooseGallery { get; set; }

        public ObservableCollection<GalleryLocation> GalleryLocations
        { get; set; }
        private readonly GalleryDataSQLiteRepository galleryDataSQLiteRepository;

        public GalleryLocationsViewmodel(GalleryDataSQLiteRepository _galleryDataSQLiteRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            galleryDataSQLiteRepository = _galleryDataSQLiteRepository;
            GalleryLocations = new ObservableCollection<GalleryLocation>(galleryDataSQLiteRepository.RetrieveGalleryLocations());

            btnCmdChooseGallery = new RelayCommand<GalleryLocation>(tmpGallery =>
            {
                if (tmpGallery == null)
                {
                    return;
                }
                NavigateToPage(AVAILABLE_PAGES.Gallery, tmpGallery);
            });
        }
    }
}
