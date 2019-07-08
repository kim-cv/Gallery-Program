using Gallery.WPF.Abstracts;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public partial class GalleryLocationsPage : NavigablePageBase
    {
        public GalleryLocationsPage(GalleryLocationsViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.OnNavigateToNewPage += NavigateToPage;
        }
    }
}
