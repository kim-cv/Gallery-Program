using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public class GalleryLocationsViewmodel
    {
        public ObservableCollection<string> GalleryLocations
        { get; set; }

        public GalleryLocationsViewmodel()
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }
        }
    }
}
