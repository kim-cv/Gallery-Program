using Gallery.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Gallery.BL
{
    public class ImageRepositoryMediator : IImageRepository
    {
        private readonly List<IImageFileRepository> imageRepositories = new List<IImageFileRepository>();

        public ImageRepositoryMediator(IImageFileRepository _imageRepository)
        {
            imageRepositories.Add(_imageRepository);
        }

        public ImageRepositoryMediator(IList<IImageFileRepository> _imageRepositories)
        {
            imageRepositories.AddRange(_imageRepositories);
        }

        public IEnumerable<BitmapSource> RetrieveImagesAsThumbs()
        {
            // Retrieve images as byte arrays
            IEnumerable<byte[]> imagesBytesArray = imageRepositories[0].RetrieveImages();

            // Map to BitmapSource
            IEnumerable<BitmapSource> bitmapSources = imagesBytesArray.Select(tmpImageBytes => ImageController.BytesToImage(tmpImageBytes));

            // Map to thumbnails
            IEnumerable<BitmapSource> bitmapSourceThumbnails = bitmapSources.Select(tmpBitmapSource => ImageController.ConvertToThumb(tmpBitmapSource, 100, 100));

            return bitmapSourceThumbnails;
        }

        public BitmapSource RetrieveImage(string imageName)
        {
            throw new NotImplementedException();
        }
    }
}