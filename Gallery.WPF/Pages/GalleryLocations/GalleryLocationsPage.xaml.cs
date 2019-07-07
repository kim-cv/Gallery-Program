using System.Windows.Controls;

namespace Gallery.WPF.Pages.GalleryLocations
{
    public partial class GalleryLocationsPage : Page
    {
        public GalleryLocationsPage(GalleryLocationsViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.OnNavigateToNewPage += NavigateToPage;
        }

        private void NavigateToPage(AVAILABLE_PAGES pageType)
        {
            Page page = PageFactory.ConstructPage(pageType);
            NavigationService.Navigate(page);
        }
    }
}
