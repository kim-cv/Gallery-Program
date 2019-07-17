using Gallery.BL;
using Gallery.Core.Interfaces;
using Gallery.DA;
using Gallery.WPF.Pages.AddGalleryLocation;
using Gallery.WPF.Pages.Gallery;
using Gallery.WPF.Pages.GalleryLocations;
using Gallery.WPF.Pages.ViewImage;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gallery.WPF
{
    public static class PageFactory
    {
        public static Page ConstructPage(AVAILABLE_PAGES pageType, object pageData)
        {
            string program_name = Properties.Resources.program_name;
            string applicationRoamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            applicationRoamingDataPath = Path.Combine(applicationRoamingDataPath, program_name);


            switch (pageType)
            {
                case AVAILABLE_PAGES.Gallery:
                    return CreatePageGallery(pageData);
                case AVAILABLE_PAGES.GalleryLocations:
                    return CreatePageGalleryLocations(applicationRoamingDataPath);
                case AVAILABLE_PAGES.AddGalleryLocation:
                    return CreatePageAddGalleryLocation(applicationRoamingDataPath);
                case AVAILABLE_PAGES.ViewImage:
                    return CreatePageViewImage(pageData);
                default:
                    return null;
            }
        }

        private static Page CreatePageViewImage(object pageData)
        {
            //if (pageData == null || pageData.GetType() != typeof(BitmapSource))
            //{
            //    throw new InvalidDataException("Page data was invalid");
            //}

            //IImageInformation imageInformation = (IImageInformation)pageData;
            IImageRepository imageRepository = (IImageRepository)pageData;
            ViewImageViewmodel viewImageViewmodel = new ViewImageViewmodel(imageRepository);
            return new ViewImagePage(viewImageViewmodel);
        }

        private static Page CreatePageGallery(object pageData)
        {
            if (pageData == null || pageData.GetType() != typeof(GalleryLocation))
            {
                throw new InvalidDataException("Page data was invalid");
            }

            GalleryLocation galleryLocation = (GalleryLocation)pageData;
            string imagesFolder = galleryLocation.Path;
            FilesystemRepository filesystemRepository = new FilesystemRepository(imagesFolder);
            IImageRepository imageRepositoryMediator = new ImageRepositoryMediator(filesystemRepository);
            GalleryViewmodel galleryViewmodel = new GalleryViewmodel(imageRepositoryMediator);
            return new GalleryPage(galleryViewmodel);
        }

        private static Page CreatePageGalleryLocations(string appRoamingDataPath)
        {
            GalleryDataSQLiteRepository galleryDataSQLiteRepository = new GalleryDataSQLiteRepository(appRoamingDataPath);
            GalleryLocationsViewmodel galleryLocationsViewmodel = new GalleryLocationsViewmodel(galleryDataSQLiteRepository);
            return new GalleryLocationsPage(galleryLocationsViewmodel);
        }

        private static Page CreatePageAddGalleryLocation(string appRoamingDataPath)
        {
            GalleryDataSQLiteRepository galleryDataSQLiteRepository = new GalleryDataSQLiteRepository(appRoamingDataPath);
            AddGalleryLocationViewmodel addGalleryLocationViewmodel = new AddGalleryLocationViewmodel(galleryDataSQLiteRepository);
            return new AddGalleryLocationPage(addGalleryLocationViewmodel);
        }
    }
}