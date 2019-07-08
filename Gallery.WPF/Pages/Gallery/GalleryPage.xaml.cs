using System.Windows.Controls;

namespace Gallery.WPF.Pages.Gallery
{
    public partial class GalleryPage : Page
    {
        public GalleryPage(GalleryViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.OnNavigateToNewPage += NavigateToPage;
        }

        private void NavigateToPage(AVAILABLE_PAGES pageType, object pageData)
        {
            Page page = PageFactory.ConstructPage(pageType, null);
            NavigationService.Navigate(page);
        }
    }
}
