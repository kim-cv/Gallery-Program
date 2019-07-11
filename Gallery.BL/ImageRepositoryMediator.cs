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

        public ImageRepositoryMediator(IImageFileRepository _imageRepository)
        {
            imageRepositories.Add(_imageRepository);
        }

        public ImageRepositoryMediator(IList<IImageFileRepository> _imageRepositories)
        {
            imageRepositories.AddRange(_imageRepositories);
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

        public void RetrieveImagesAsThumbs()
        {
            // Retrieve images as byte arrays
            FileInfo[] fileInfos = imageRepositories[0].RetrieveImages();
            string repositoryUid = imageRepositories[0].uid;

            // Using parallel convert all byte arrays to thumbnail bitmapsource and invoke event
            fileInfos
                .AsParallel()
                .ForAll(tmpFileInfo =>
                {
                    var image = new ImageInformation(repositoryUid, tmpFileInfo);
                    OnNewImage?.Invoke(image);
                });
        }

        public IImageInformation RetrieveImage(string imageName)
        {
            throw new NotImplementedException();
        }
    }
}