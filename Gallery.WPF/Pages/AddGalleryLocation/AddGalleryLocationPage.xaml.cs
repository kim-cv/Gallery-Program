using System.Windows.Controls;

namespace Gallery.WPF.Pages.AddGalleryLocation
{
    public partial class AddGalleryLocationPage : Page
    {
        public AddGalleryLocationPage(AddGalleryLocationViewmodel viewModel)
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
