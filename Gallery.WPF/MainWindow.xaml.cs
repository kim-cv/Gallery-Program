using Gallery.BL;
using Gallery.Core.Interfaces;
using Gallery.DA;
using Gallery.WPF.Views.Gallery;
using System;
using System.IO;
using System.Windows;

namespace Gallery.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            VerifyApplicationDataRoamingFolderExist();

            // Construct gallery page with constructor dependency injection
            string imagesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            FilesystemRepository filesystemRepository = new FilesystemRepository(imagesFolder);
            IImageRepository imageRepositoryMediator = new ImageRepositoryMediator(filesystemRepository);
            GalleryViewmodel galleryViewmodel = new GalleryViewmodel(imageRepositoryMediator);
            GalleryPage galleryPage = new GalleryPage(galleryViewmodel);

            // Show page
            Content = galleryPage;
        }

        private void VerifyApplicationDataRoamingFolderExist()
        {
            string program_name = Properties.Resources.program_name;

            string applicationRoamingDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            applicationRoamingDataPath = Path.Combine(applicationRoamingDataPath, program_name);

            if (!Directory.Exists(applicationRoamingDataPath))
            {
                Directory.CreateDirectory(applicationRoamingDataPath);
            }
        }
    }
}
