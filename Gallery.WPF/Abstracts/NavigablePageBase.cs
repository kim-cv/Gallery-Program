using System.Windows.Controls;

namespace Gallery.WPF.Abstracts
{
    public abstract class NavigablePageBase : Page
    {
        public NavigablePageBase()
        {

        }

        protected void NavigateToPage(AVAILABLE_PAGES pageType, object pageData)
        {
            Page page = PageFactory.ConstructPage(pageType, pageData);
            NavigationService.Navigate(page);
        }
    }
}
