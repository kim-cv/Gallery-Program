using Gallery.WPF.EventHandlers;

namespace Gallery.WPF.Interfaces
{
    public interface IViewmodel
    {
        event NavigateToPageEventHandler OnNavigateToNewPage;
    }
}
