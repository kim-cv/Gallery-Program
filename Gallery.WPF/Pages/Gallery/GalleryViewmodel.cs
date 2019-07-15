using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Gallery.WPF.Interfaces;
using System.Windows;
using System;
using Gallery.Core.Interfaces;

namespace Gallery.WPF.Pages.Gallery
{
    public class GalleryViewmodel : IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand btnCmdChooseImage { get; set; }
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

        public ObservableCollection<IImageInformation> Images { get; set; } = new ObservableCollection<IImageInformation>();
        private readonly IImageRepository imageRepositoryMediator;


        public GalleryViewmodel(IImageRepository _imageRepositoryMediator)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            btnCmdChooseImage = new RelayCommand<IImageInformation>(tmpImage =>
            {
                cmdChooseImage(tmpImage);
            });

            imageRepositoryMediator = _imageRepositoryMediator;

            _imageRepositoryMediator.OnNewImage += OnNewImage;
            imageRepositoryMediator.RetrieveImagesAsThumbs();
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private async void OnNewImage(IImageInformation imageInformation)
        {
            await imageInformation.RetrieveThumb();

            // Force this to run on UI thread because this method is called from events working on other threads
            await Application
                .Current
                .Dispatcher
                .BeginInvoke(new Action(() =>
            {
                Images.Add(imageInformation);
            }));
        }

        private void cmdChooseImage(IImageInformation image)
        {
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.ViewImage, image);
        }
    }
}