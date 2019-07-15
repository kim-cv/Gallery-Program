using Gallery.Core.Interfaces;
using Gallery.WPF.Interfaces;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Gallery.WPF.Pages.ViewImage
{
    public class ViewImageViewmodel : IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;
        public ICommand btnCmdPreviousImage { get; set; }
        public ICommand btnCmdNextImage { get; set; }

        public IImageInformation image { get; }

        public ViewImageViewmodel(IImageInformation _image)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            image = _image;
            image.RetrieveFullImage();

            btnCmdPreviousImage = new RelayCommand(cmdPreviousImage);
            btnCmdNextImage = new RelayCommand(cmdNextImage);
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void cmdPreviousImage()
        {

        }

        private void cmdNextImage()
        {

        }
    }
}
