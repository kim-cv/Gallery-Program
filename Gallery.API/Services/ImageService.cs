using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Gallery.API.Models;
using Gallery.API.Entities;
using Gallery.API.Interfaces;
using Gallery.API.Helpers;

namespace Gallery.API.Services
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IFileSystemRepository _fileSystemRepository;

        public ImageService(IImageRepository imageRepository, IFileSystemRepository fileSystemRepository)
        {
            _imageRepository = imageRepository;
            _fileSystemRepository = fileSystemRepository;
        }

        public async Task<bool> DoesImageExistAsync(Guid imageId)
        {
            ImageEntity image = await _imageRepository.GetImage(imageId);
            return (image != null);
        }

        public async Task<bool> IsImageInsideGalleryAsync(Guid imageId, Guid galleryId)
        {
            ImageEntity image = await _imageRepository.GetImage(imageId);
            return (image.fk_gallery == galleryId);
        }

        public async Task<ImageDTO> CreateImageAsync(Guid userId, Guid galleryId, ImageCreationDTO imageCreationDTO)
        {
            ImageEntity entity = imageCreationDTO.ToImageEntity();
            entity.fk_gallery = galleryId;

            ImageEntity addedEntity = await _imageRepository.PostImage(entity);

            if (_imageRepository.Save() == false)
            {
                throw new Exception();
            }

            IFormFile formFile = imageCreationDTO.formFile;
            if (formFile.Length > 0)
            {
                string extension = Path.GetExtension(formFile.FileName);
                string filename = addedEntity.Id.ToString();

                byte[] formfileBytes;
                using (Stream stream = formFile.OpenReadStream())
                {
                    formfileBytes = new byte[stream.Length];
                    await stream.ReadAsync(formfileBytes, 0, (int)stream.Length);
                }

                await _fileSystemRepository.SaveFile(formfileBytes, filename, extension);
            }

            return addedEntity.ToImageDto();
        }

        public async Task<byte[]> GetImageAsync(Guid imageId, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio)
        {
            ImageEntity image = await _imageRepository.GetImage(imageId);

            byte[] imgData = await _fileSystemRepository.RetrieveFile(image.Id.ToString(), image.Extension);

            if (thumb == true)
            {
                imgData = GenerateThumb(imgData, (int)thumbWidth, (int)thumbHeight, (bool)keepAspectRatio);
            }

            return imgData;
        }

        public async Task<IEnumerable<byte[]>> GetImagesInGalleryAsync(Guid galleryId, Pagination pagination, bool thumb, int? thumbWidth, int? thumbHeight, bool? keepAspectRatio)
        {
            IEnumerable<ImageEntity> imageEntities = await _imageRepository.GetImages(galleryId, pagination);

            IEnumerable<Task<byte[]>> tasks = imageEntities.Select(async tmpEntity =>
            {
                byte[] imgData = await _fileSystemRepository.RetrieveFile(tmpEntity.Id.ToString(), tmpEntity.Extension);

                if (thumb == true)
                {
                    imgData = GenerateThumb(imgData, (int)thumbWidth, (int)thumbHeight, (bool)keepAspectRatio);
                }

                return imgData;
            });

            IEnumerable<byte[]> imgDatas = await Task.WhenAll(tasks);

            return imgDatas;
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            ImageEntity imageEntity = await _imageRepository.GetImage(imageId);

            // Delete from filesystem
            _fileSystemRepository.DeleteFile(imageEntity.Id.ToString(), imageEntity.Extension);

            // Delete from DB
            await _imageRepository.DeleteImage(imageId);

            if (_imageRepository.Save() == false)
            {
                throw new Exception();
            }
        }

        public byte[] GenerateThumb(byte[] imageData, int maxWidth, int maxHeight, bool keepAspectRatio)
        {
            using (Image<Rgba32> image = Image.Load(imageData))
            using (var ms = new MemoryStream())
            {
                int imgWidth = image.Width;
                int imgHeight = image.Height;

                (int ThumbWidth, int ThumbHeight) = AspectRatio(imgWidth, imgHeight, maxWidth, maxHeight, keepAspectRatio);

                image.Mutate(x => x
                     .Resize(ThumbWidth, ThumbHeight));
                image.SaveAsJpeg(ms);
                return ms.ToArray();
            }
        }

        private (int ThumbWidth, int ThumbHeight) AspectRatio(double currentWidth, double currentHeight, double maxWidth, double maxHeight, bool keepAspectRatio)
        {
            // Used for aspect ratio
            double ratio = Math.Min(maxWidth / currentWidth, maxHeight / currentHeight);

            double calculatedWidth = maxWidth;
            double calculatedHeight = maxHeight;

            if (keepAspectRatio)
            {
                if (currentWidth > maxWidth)
                {
                    calculatedHeight = currentHeight * ratio;
                }

                if (currentHeight > maxHeight)
                {
                    calculatedWidth = currentWidth * ratio;
                }
            }

            int finalWidth = Convert.ToInt32(calculatedWidth);
            int finalHeight = Convert.ToInt32(calculatedHeight);

            return (ThumbWidth: finalWidth, ThumbHeight: finalHeight);
        }
    }
}
