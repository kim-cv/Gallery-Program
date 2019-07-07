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

        private void NavigateToPage(AVAILABLE_PAGES pageType)
        {
            Page page = PageFactory.ConstructPage(pageType);
            NavigationService.Navigate(page);
        }
    }
}
