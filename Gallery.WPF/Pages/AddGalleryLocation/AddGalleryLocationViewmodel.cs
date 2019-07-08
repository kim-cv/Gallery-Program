using Gallery.BL;
using Gallery.WPF.Interfaces;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace Gallery.WPF.Pages.AddGalleryLocation
{
    public class AddGalleryLocationViewmodel : INotifyPropertyChanged, IViewmodel
    {
        // Events & Commands
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand btnCmdChooseLocationPath { get; set; }
        public ICommand btnCmdSave { get; set; }
        public event EventHandlers.NavigateToPageEventHandler OnNavigateToNewPage;


        public string selectedPath { get; set; }
        public string galleryName { get; set; }
        private readonly GalleryDataSQLiteRepository galleryDataSQLiteRepository;

        public AddGalleryLocationViewmodel(GalleryDataSQLiteRepository _galleryDataSQLiteRepository)
        {
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                return;
            }

            galleryDataSQLiteRepository = _galleryDataSQLiteRepository;

            btnCmdChooseLocationPath = new RelayCommand(OpenFolderDialog);
            btnCmdSave = new RelayCommand(SaveGalleryLocation);
        }

        protected void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        private void SaveGalleryLocation()
        {
            galleryDataSQLiteRepository.AddGalleryLocation(galleryName, selectedPath);
            OnNavigateToNewPage?.Invoke(AVAILABLE_PAGES.GalleryLocations, null);
        }

        private void OpenFolderDialog()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    selectedPath = folderBrowserDialog.SelectedPath;
                } else
                {
                    selectedPath = "";
                }
            }

            NotifyPropertyChanged("selectedPath");
        }
    }
}
