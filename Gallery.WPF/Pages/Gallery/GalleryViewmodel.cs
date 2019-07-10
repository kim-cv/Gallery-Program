using Gallery.Core.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Gallery.WPF.Interfaces;
using System.Windows;
using System;

namespace Gallery.WPF.Pages.Gallery
{
    public class GalleryViewmodel : IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand btnCmdChooseImage { get; set; }
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;

        public ObservableCollection<BitmapSource> Images { get; set; } = new ObservableCollection<BitmapSource>();
        private readonly IImageRepository imageRepositoryMediator;

        public GalleryViewmodel(IImageRepository _imageRepositoryMediator)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            btnCmdChooseImage = new RelayCommand<BitmapSource>(tmpImage =>
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

        private void OnNewImage(BitmapSource bitmapSource)
        {
            // Force this to run on UI thread because this method is called from events working on other threads
            Application
                .Current
                .Dispatcher
                .BeginInvoke(new Action(() =>
            {
                Images.Add(bitmapSource);
            }));
        }

        private void cmdChooseImage(BitmapSource image)
        {
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.AddGalleryLocation, null);
        }
    }
}