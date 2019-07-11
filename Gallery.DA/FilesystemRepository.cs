using Gallery.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gallery.DA
{
    public class FilesystemRepository : IImageFileRepository
    {
        public string uid { get; }
        public string directoryUrl { get; }
        private readonly FileInfo[] fileInfos;

        public FilesystemRepository(string directoryUrl)
        {
            // Check if directory exist
            if (Directory.Exists(directoryUrl) == false)
            {
                throw new DirectoryNotFoundException();
            }

            // Generate random string uid
            uid = Guid.NewGuid().ToString("n");

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
                .Select(tmpFilenameWithPath => new FileInfo(tmpFilenameWithPath));

            // Set filenames
            fileInfos = mappedToFileInfos.ToArray();
        }

        //public async Task<IEnumerable<FileInfo[]>> RetrieveImages()
        //{
        //    IList<byte[]> imageByteArray = new List<byte[]>(fileInfos.Length);

        //    foreach (FileInfo tmpFileInfo in fileInfos)
        //    {
        //        byte[] imageBytes;
        //        using (FileStream stream = File.Open(tmpFileInfo.FullName, FileMode.Open))
        //        {
        //            imageBytes = new byte[stream.Length];
        //            await stream.ReadAsync(imageBytes, 0, (int)stream.Length);
        //            imageByteArray.Add(imageBytes);
        //        }
        //    }

        //    return imageByteArray;
        //}

        public FileInfo[] RetrieveImages()
        {
            return fileInfos;
        }

        //public byte[] RetrieveImage(string imageName)
        public FileInfo RetrieveImage(string imageName)
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

            // return File.ReadAllBytes(fileInfo.FullName);
            return fileInfo;
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
