using System.Windows.Controls;

namespace Gallery.WPF.Pages.AddGalleryLocation
{
    public partial class AddGalleryLocationPage : Page
    {
        public AddGalleryLocationPage(AddGalleryLocationViewmodel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
