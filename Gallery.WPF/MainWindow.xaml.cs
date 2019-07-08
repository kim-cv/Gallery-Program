using Gallery.BL;
using Gallery.Core.Interfaces;
using Gallery.DA;
using Gallery.WPF.Views.Gallery;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Gallery.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            VerifyApplicationDataRoamingFolderExist();

            // Construct first page
            Page page = PageFactory.ConstructPage(AVAILABLE_PAGES.GalleryLocations, null);

            // Show page
            _mainFrame.Navigate(page);
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
