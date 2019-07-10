using System.Windows.Controls;

namespace Gallery.WPF.Pages.ViewImage
{
    public partial class ViewImagePage : Page
    {
        public ViewImagePage(ViewImageViewmodel viewModel)
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
