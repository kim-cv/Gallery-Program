using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
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
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            galleryDataSQLiteRepository = _galleryDataSQLiteRepository;
            GalleryLocations = new ObservableCollection<GalleryLocation>(galleryDataSQLiteRepository.RetrieveGalleryLocations());

            btnCmdChooseGallery = new RelayCommand<GalleryLocation>(tmpGallery =>
            {
                if (tmpGallery == null)
                {
                    ((IViewmodel)this).NavigationErrorPopup("Unable to view gallery.");
                }

                try
                {
                    NavigateToPage(AVAILABLE_PAGES.Gallery, tmpGallery);
                }
                catch (Exception ex)
                {
                    ((IViewmodel)this).NavigationErrorPopup("Unable to view gallery.");
                }
            });
        }
    }
}
