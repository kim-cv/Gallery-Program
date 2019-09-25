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

namespace Gallery.WPF
{
    public static class PageFactory
    {
        public static Page ConstructPage(AVAILABLE_PAGES pageType, object pageData)
        {
            string program_name = Properties.Resources.program_name;
            string applicationRoamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            applicationRoamingDataPath = Path.Combine(applicationRoamingDataPath, program_name);


            return pageType switch
            {
                AVAILABLE_PAGES.Gallery => CreatePageGallery(pageData),
                AVAILABLE_PAGES.GalleryLocations => CreatePageGalleryLocations(applicationRoamingDataPath),
                AVAILABLE_PAGES.AddGalleryLocation => CreatePageAddGalleryLocation(applicationRoamingDataPath),
                AVAILABLE_PAGES.ViewImage => CreatePageViewImage(pageData),
                _ => null,
            };
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
            if (pageData == null)
            {
                throw new InvalidDataException("Page data was invalid");
            }

            if (pageData.GetType() == typeof(GalleryLocation))
            {
                GalleryLocation galleryLocation = (GalleryLocation)pageData;
                string imagesFolder = galleryLocation.Path;
                FilesystemRepository filesystemRepository = new FilesystemRepository(imagesFolder);
                IImageRepository imageRepositoryMediator = new ImageRepositoryMediator(filesystemRepository);
                GalleryViewmodel galleryViewmodel = new GalleryViewmodel(imageRepositoryMediator);
                return new GalleryPage(galleryViewmodel);
            }

            if (pageData is IImageRepository)
            {
                IImageRepository imageRepositoryMediator = (IImageRepository)pageData;
                GalleryViewmodel galleryViewmodel = new GalleryViewmodel(imageRepositoryMediator);
                return new GalleryPage(galleryViewmodel);
            }

            throw new InvalidDataException("Page data was invalid");
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