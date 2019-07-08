using Gallery.BL;
using Gallery.Core.Interfaces;
using Gallery.DA;
using Gallery.WPF.Pages.AddGalleryLocation;
using Gallery.WPF.Pages.Gallery;
using Gallery.WPF.Pages.GalleryLocations;
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


            switch (pageType)
            {
                case AVAILABLE_PAGES.Gallery:
                    return CreatePageGallery(pageData);
                case AVAILABLE_PAGES.GalleryLocations:
                    return CreatePageGalleryLocations(applicationRoamingDataPath);
                case AVAILABLE_PAGES.AddGalleryLocation:
                    return CreatePageAddGalleryLocation(applicationRoamingDataPath);
                default:
                    return null;
            }
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