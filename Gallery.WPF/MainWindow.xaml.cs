using Gallery.WPF.Views.Gallery;
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
            GalleryPage galleryPage = new GalleryPage();
            galleryPage.DataContext = new GalleryViewmodel();
            Content = galleryPage;
        }
    }
}
