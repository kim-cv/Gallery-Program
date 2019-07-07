using System.Windows.Controls;

namespace Gallery.WPF.Views.Gallery
{
    public partial class GalleryPage : Page
    {
        public GalleryPage(GalleryViewmodel viewModel)
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
