﻿using Gallery.Core.Interfaces;
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
        public ICommand btnCmdBackToGallery { get; set; }
        public ICommand btnCmdPreviousImage { get; set; }
        public ICommand btnCmdNextImage { get; set; }

        public IImageInformation image { get; set; }
        private IImageRepositoryCache imageRepository { get; }

        public ViewImageViewmodel(IImageRepositoryCache _imageRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            imageRepository = _imageRepository;
            image = imageRepository.CurrentLargeImage;
            image.RetrieveFullImage();

            btnCmdBackToGallery = new RelayCommand(cmdBackToGallery);
            btnCmdPreviousImage = new RelayCommand(cmdPreviousImage);
            btnCmdNextImage = new RelayCommand(cmdNextImage);
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void cmdBackToGallery()
        {
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.Gallery, imageRepository);
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
