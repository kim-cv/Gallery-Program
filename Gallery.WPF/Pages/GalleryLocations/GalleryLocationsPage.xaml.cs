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

        private void NavigateToPage(AVAILABLE_PAGES pageType, object pageData)
        {
            Page page = PageFactory.ConstructPage(pageType, pageData);
            NavigationService.Navigate(page);
        }
    }
}
