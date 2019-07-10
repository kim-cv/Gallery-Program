using Gallery.WPF.Interfaces;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Gallery.WPF.Pages.ViewImage
{
    public class ViewImageViewmodel : IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

        public BitmapSource image { get; }

        public ViewImageViewmodel(BitmapSource _image)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            image = _image;
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
