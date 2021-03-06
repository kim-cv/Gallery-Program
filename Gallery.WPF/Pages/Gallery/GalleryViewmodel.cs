﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Gallery.WPF.Interfaces;
using Gallery.Core.Interfaces;
using System.Collections.Generic;

namespace Gallery.WPF.Pages.Gallery
{
    public class GalleryViewmodel : AbstractLeftMenu, IViewmodel, INotifyPropertyChanged
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand btnCmdChooseImage { get; set; }

        public ObservableCollection<IImageInformation> Images { get; set; } = new ObservableCollection<IImageInformation>();
        private readonly IImageRepositoryCache imageRepositoryCache;

        private int numOfCurrentShowingItems = 0;
        private readonly int numOfNewImagesPerRequest = 40;
        private bool isCurrentlyLoading = false;
        public int numPreviouslyLoadedImages = 0;


        public GalleryViewmodel(IImageRepositoryCache _imageRepositoryCache)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            btnCmdChooseImage = new RelayCommand<IImageInformation>(tmpImage =>
            {
                cmdChooseImage(tmpImage);
            });

            imageRepositoryCache = _imageRepositoryCache;

            // Scroll to previous image
            if (imageRepositoryCache.CurrentLargeImage != null)
            {
                var returnedFromBigImage = imageRepositoryCache.CurrentLargeImage;
                var imagesPreviouslyLoaded = imageRepositoryCache.RetrieveImagesUpTo(returnedFromBigImage);
                foreach (IImageInformation item in imagesPreviouslyLoaded)
                {
                    item.RetrieveThumb();
                    Images.Add(item);
                }
                numOfCurrentShowingItems += imagesPreviouslyLoaded.Count();
                numPreviouslyLoadedImages = imagesPreviouslyLoaded.Count();
            }

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
            if (image == null) return;

            imageRepositoryCache.CurrentLargeImage = image;
            NavigateToPage(AVAILABLE_PAGES.ViewImage, imageRepositoryCache);
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
            IEnumerable<IImageInformation> newImages = imageRepositoryCache.RetrieveImages(numOfCurrentShowingItems, numOfNewImagesPerRequest);

            foreach (IImageInformation item in newImages)
            {
                item.RetrieveThumb();
                Images.Add(item);
            }

            numOfCurrentShowingItems += newImages.Count();
            isCurrentlyLoading = false;
        }
    }
}