using System.Windows;
using Gallery.WPF.EventHandlers;

namespace Gallery.WPF.Interfaces
{
    public interface IViewmodel
    {
        event NavigateToPageEventHandler OnNavigateToNewPage;
        public void NavigationErrorPopup(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
