using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Gallery.BL
{
    public class FilesystemRepository
    {
        public readonly string directoryUrl;
        private readonly FileInfo[] fileInfos;

        public FilesystemRepository(string directoryUrl)
        {
            // Check if directory exist
            if (Directory.Exists(directoryUrl) == false)
            {
                throw new DirectoryNotFoundException();
            }

            // Set directory url
            this.directoryUrl = directoryUrl;

            // Get all filenames in directory
            string[] filenamesWithPath = Directory.GetFiles(directoryUrl, "*.*", SearchOption.TopDirectoryOnly);

            // Filter to only get filenames with certain extension
            IList<string> allowedExtension = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
            IEnumerable<string> filteredFilenamesWithPath = filenamesWithPath
                .Where(tmpFilename => allowedExtension.Contains(Path.GetExtension(tmpFilename)));

            // Map to fileinfo
            IEnumerable<FileInfo> mappedToFileInfos = filteredFilenamesWithPath
                .Select(tmpFilenameWithPath =>
                {
                    return new FileInfo(tmpFilenameWithPath);
                });

            // Set filenames
            fileInfos = mappedToFileInfos.ToArray();
        }

        public IList<Image> RetrieveImagesAsThumbs()
        {
            return fileInfos.Select(tmpFileInfo =>
            {
                byte[] imageBytes = File.ReadAllBytes(tmpFileInfo.FullName);
                Image img = ImageController.BytesToImage(imageBytes);
                return img;
            }).ToList();
        }

        public Image RetrieveImage(string imageName)
        {
            bool imageNameExist = DoesFileInfoExistWithFileName(imageName);
            if (imageNameExist == false)
            {
                return null;
            }

            FileInfo fileInfo = RetrieveFileInfoFromFilename(imageName);
            if (fileInfo == null)
            {
                return null;
            }

            byte[] imageBytes = File.ReadAllBytes(fileInfo.FullName);
            return ImageController.BytesToImage(imageBytes);
        }

        private FileInfo RetrieveFileInfoFromFilename(string imageName)
        {
            return fileInfos.FirstOrDefault(tmpFileInfo => tmpFileInfo.Name == imageName);
        }

        private bool DoesFileInfoExistWithFileName(string imageName)
        {
            return fileInfos.Any(item => item.Name == imageName);
        }
    }
}
