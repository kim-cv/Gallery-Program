using System.Windows.Controls;
using System.Windows.Media;

namespace Gallery.WPF.Pages.Gallery
{
    public partial class GalleryPage : Page
    {
        private readonly GalleryViewmodel viewModel;
        public GalleryPage(GalleryViewmodel _viewModel)
        {
            InitializeComponent();
            viewModel = _viewModel;
            DataContext = viewModel;

            viewModel.OnNavigateToNewPage += NavigateToPage;
        }

        private void NavigateToPage(AVAILABLE_PAGES pageType, object pageData)
        {
            Page page = PageFactory.ConstructPage(pageType, pageData);
            NavigationService.Navigate(page);
        }


        private void listview_images_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Get the border of the listview (first element of a listview)
            Decorator border = VisualTreeHelper.GetChild(listview_images, 0) as Decorator;
            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;


            double verticalOffset = scrollViewer.VerticalOffset;
            double maxVerticalOffset = scrollViewer.ScrollableHeight; //scrollViewer.ExtentHeight - scrollViewer.ViewportHeight;

            if (maxVerticalOffset < 0 ||
                verticalOffset == maxVerticalOffset)
            {
                // Scrolled to bottom
                viewModel.LoadMoreThumbs();
            }
            else
            {
                // Not scrolled to bottom
            }
        }
    }
}
