using Gallery.BL;
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

            // Construct gallery page with dependency injection
            string imagesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            FilesystemRepository filesystemRepository = new FilesystemRepository(imagesFolder);
            GalleryViewmodel galleryViewmodel = new GalleryViewmodel(filesystemRepository);
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
