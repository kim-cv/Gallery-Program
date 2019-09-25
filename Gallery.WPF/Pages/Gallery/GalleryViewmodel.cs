using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Gallery.WPF.Interfaces;
using Gallery.Core.Interfaces;

namespace Gallery.WPF.Pages.Gallery
{
    public class GalleryViewmodel : AbstractLeftMenu, IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand btnCmdChooseImage { get; set; }

        public ObservableCollection<IImageInformation> Images { get; set; } = new ObservableCollection<IImageInformation>();
        private readonly IImageRepository imageRepositoryMediator;

        private int numOfCurrentShowingItems = 0;
        private readonly int numOfNewImagesPerRequest = 20;
        private bool isCurrentlyLoading = false;


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

            //_imageRepositoryMediator.OnNewImage += OnNewImage;
            //imageRepositoryMediator.RetrieveImagesAsThumbs();
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        //private async void OnNewImage(IImageInformation imageInformation)
        //{
        //    await imageInformation.RetrieveThumb();

        //    Images.Add(imageInformation);
        //    // Force this to run on UI thread because this method is called from events working on other threads
        //    //await Application
        //    //    .Current
        //    //    ?.Dispatcher
        //    //    ?.BeginInvoke(new Action(() =>
        //    //{
        //    //    Images.Add(imageInformation);
        //    //}));
        //}

        private void cmdChooseImage(IImageInformation image)
        {
            imageRepositoryMediator.CurrentLargeImage = image;
            NavigateToPage(AVAILABLE_PAGES.ViewImage, imageRepositoryMediator);
        }

        public void LoadMoreThumbs()
        {
            // Dont load more if loading is already in process
            if (isCurrentlyLoading)
            {
                return;
            }

            isCurrentlyLoading = true;
            // Get more images
            IImageInformation[] newImages = imageRepositoryMediator.RetrieveImages(numOfCurrentShowingItems, numOfNewImagesPerRequest).ToArray();
            foreach (IImageInformation item in newImages)
            {
                item.RetrieveThumb();
                Images.Add(item);
            }
            numOfCurrentShowingItems += newImages.Length;
            isCurrentlyLoading = false;
        }
    }
}