using Gallery.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gallery.BL
{
    public class ImageRepositoryMediator : IImageRepository
    {
        public event NewImageEventHandler OnNewImage;

        private readonly List<IImageFileRepository> imageRepositories = new List<IImageFileRepository>();
        private readonly List<FileInfo> fileInfos = new List<FileInfo>();

        public ImageRepositoryMediator(IImageFileRepository _imageRepository)
        {
            imageRepositories.Add(_imageRepository);
            IEnumerable<FileInfo[]> tmpFileInfos = RetrieveAllFileInfos();
            MergeFileInfos(tmpFileInfos);
        }

        public ImageRepositoryMediator(IList<IImageFileRepository> _imageRepositories)
        {
            imageRepositories.AddRange(_imageRepositories);
            IEnumerable<FileInfo[]> tmpFileInfos = RetrieveAllFileInfos();
            MergeFileInfos(tmpFileInfos);
        }

        private IEnumerable<FileInfo[]> RetrieveAllFileInfos()
        {
            // Get FileInfos from each repository
            return imageRepositories.Select(tmpImageRepository => tmpImageRepository.RetrieveImages());
        }
        private void MergeFileInfos(IEnumerable<FileInfo[]> repositoryFileInfos)
        {
            // Merge all FileInfo arrays into a single list
            foreach (FileInfo[] tmpFileInfos in repositoryFileInfos)
            {
                fileInfos.AddRange(tmpFileInfos);
            }
        }

        //public async void RetrieveImagesAsThumbs()
        //{
        //    // Retrieve images as byte arrays
        //    IEnumerable<byte[]> imagesBytesArray = await imageRepositories[0].RetrieveImages();

        //    // Using parallel convert all byte arrays to thumbnail bitmapsource and invoke event
        //    imagesBytesArray
        //        .AsParallel()
        //        .ForAll(async tmpImageBytes =>
        //        {
        //            BitmapSource bitmapSource = await ImageController.BytesToImage(tmpImageBytes);
        //            BitmapSource thumbBitmapSource = await ImageController.ConvertToThumb(bitmapSource, 100, 100);
        //            OnNewImage?.Invoke(thumbBitmapSource);
        //        });
        //}

        //public void RetrieveImagesAsThumbs()
        //{
        //    //string repositoryUid = imageRepositories[0].uid;

        //    //// Using parallel convert all byte arrays to thumbnail bitmapsource and invoke event
        //    //fileInfos
        //    //    .AsParallel()
        //    //    .ForAll(tmpFileInfo =>
        //    //    {
        //    //        var image = new ImageInformation(repositoryUid, tmpFileInfo);
        //    //        OnNewImage?.Invoke(image);
        //    //    });

        //    foreach (var tmpFileInfo in fileInfos)
        //    {
        //        OnNewImage?.Invoke(new ImageInformation("testUid", tmpFileInfo));
        //    }
        //}

        public IEnumerable<IImageInformation> RetrieveImages(int from, int to)
        {
            IEnumerable<FileInfo> tmpFileInfos = fileInfos.Skip(from).Take(to);
            IEnumerable<IImageInformation> tmpImageInformations = tmpFileInfos.Select(tmpFileInfo => new ImageInformation("testUid", tmpFileInfo));
            return tmpImageInformations;
        }

        public IEnumerable<IImageInformation> RetrieveImagesUpTo(IImageInformation image)
        {
            int index = fileInfos.FindIndex(tmp => tmp.FullName == image.fileInfo.FullName);
            int from = 0;
            int to = index + 1;
            return RetrieveImages(from, to);
        }

        public IImageInformation CurrentLargeImage { get; set; }
        public IImageInformation NextImage()
        {
            int currentIndex = fileInfos.FindIndex(tmpFileInfo => tmpFileInfo == CurrentLargeImage.fileInfo);
            if (currentIndex == -1)
            {
                return null;
            }

            int nextIndex = currentIndex += 1;
            FileInfo nextFileInfo;
            try
            {
                nextFileInfo = fileInfos[nextIndex];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return null;
            }

            IImageInformation nextImage = new ImageInformation("testUid", nextFileInfo);
            CurrentLargeImage = nextImage;
            return nextImage;
        }

        public IImageInformation PreviousImage()
        {
            int currentIndex = fileInfos.FindIndex(tmpFileInfo => tmpFileInfo == CurrentLargeImage.fileInfo);
            if (currentIndex == -1)
            {
                return null;
            }

            int previousIndex = currentIndex -= 1;
            FileInfo previousFileInfo;
            try
            {
                previousFileInfo = fileInfos[previousIndex];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return null;
            }

            IImageInformation previousImage = new ImageInformation("testUid", previousFileInfo);
            CurrentLargeImage = previousImage;
            return previousImage;
        }
    }
}