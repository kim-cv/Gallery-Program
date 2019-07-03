using System.Windows.Controls;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public partial class GalleryLocationsPage : Page
    {
        public GalleryLocationsPage(GalleryLocationsViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
