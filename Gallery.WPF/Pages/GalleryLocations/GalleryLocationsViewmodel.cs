using Gallery.BL;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public class GalleryLocationsViewmodel
    {
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
        }
    }
}
