using System.Windows.Controls;

namespace Gallery.WPF.Views.Gallery
{
    public partial class GalleryPage : Page
    {
        public GalleryPage(GalleryViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
