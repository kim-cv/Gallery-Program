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

        public IImageInformation image { get; set; }
        private IImageRepository imageRepository { get; }

        public ViewImageViewmodel(IImageRepository _imageRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            imageRepository = _imageRepository;
            image = imageRepository.CurrentLargeImage;
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
            image = imageRepository.PreviousImage();
            if (image != null)
            {
                image.RetrieveFullImage();
                NotifyPropertyChanged("image");
            }
        }

        private void cmdNextImage()
        {
            image = imageRepository.NextImage();
            if (image != null)
            {
                image.RetrieveFullImage();
                NotifyPropertyChanged("image");
            }
        }
    }
}
