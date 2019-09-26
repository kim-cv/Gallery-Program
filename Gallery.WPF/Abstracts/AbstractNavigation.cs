namespace Gallery.WPF
{
    public abstract class AbstractNavigation
    {
        // Events
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

        public void NavigateToPage(AVAILABLE_PAGES page, object data)
        {
            OnNavigateToNewPage?.Invoke(page, data);
        }
    }
}
