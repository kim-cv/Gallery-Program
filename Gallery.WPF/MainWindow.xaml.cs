using Gallery.WPF.Views.Gallery;
using System;
using System.IO;
using System.Windows;

namespace Gallery.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            VerifyApplicationDataRoamingFolderExist();

            GalleryPage galleryPage = new GalleryPage();
            galleryPage.DataContext = new GalleryViewmodel();
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
