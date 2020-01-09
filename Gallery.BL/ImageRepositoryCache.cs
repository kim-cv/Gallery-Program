using Gallery.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gallery.BL
{
    public class ImageRepositoryCache : ImageRepositoryMediator, IImageRepositoryCache
    {
        private readonly List<IImageInformation> ImagesCache = new List<IImageInformation>();
        public IImageInformation CurrentLargeImage { get; set; }

        public ImageRepositoryCache(IImageFileRepository _imageRepository) : base(_imageRepository)
        {
        }

        public ImageRepositoryCache(IList<IImageFileRepository> _imageRepositories) : base(_imageRepositories)
        {
        }

        public override IEnumerable<IImageInformation> RetrieveImages(int from, int to)
        {
            IEnumerable<IImageInformation> images = base.RetrieveImages(from, to);
            IList<IImageInformation> imagesToReturn = new List<IImageInformation>(images.Count());

            // Check if images exist in cache already, if not add them to cache
            foreach (var tmpImage in images)
            {
                if (DoesImageExistInCache(tmpImage) == false)
                {
                    ImagesCache.Add(tmpImage);
                    imagesToReturn.Add(tmpImage);
                }
                else
                {
                    IImageInformation result = ImagesCache.Find(tmp => tmp.fileInfo.FullName == tmpImage.fileInfo.FullName);
                    imagesToReturn.Add(result);
                }
            }

            return imagesToReturn;
        }

        public override IEnumerable<IImageInformation> RetrieveImagesUpTo(IImageInformation image)
        {
            IEnumerable<IImageInformation> images = base.RetrieveImagesUpTo(image);
            IList<IImageInformation> imagesToReturn = new List<IImageInformation>(images.Count());

            // Check if images exist in cache already, if not add them to cache
            foreach(var tmpImage in images)
            {
                if (DoesImageExistInCache(tmpImage) == false)
                {
                    ImagesCache.Add(tmpImage);
                    imagesToReturn.Add(tmpImage);
                } else
                {
                    IImageInformation result = ImagesCache.Find(tmp => tmp.fileInfo.FullName == tmpImage.fileInfo.FullName);
                    imagesToReturn.Add(result);
                }
            }

            return imagesToReturn;
        }

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

        private bool DoesImageExistInCache(IImageInformation image)
        {
            IImageInformation result =  ImagesCache.Find(tmp => tmp.fileInfo.FullName == image.fileInfo.FullName);
            return result == null ? false : true;
        }
    }
}