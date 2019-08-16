using System.IO;
using Gallery.API.Entities;

namespace Gallery.API.Models
{
    public static class ImageDtoToEntityMapper
    {
        public static ImageEntity ToImageEntity(this ImageDTO imageDto)
        {
            return new ImageEntity()
            {
                Id = imageDto.Id,
                fk_gallery = imageDto.galleryId,
                Name = imageDto.Name,
                Extension = imageDto.Extension,
                SizeInBytes = imageDto.SizeInBytes
            };
        }

        public static ImageEntity ToImageEntity(this ImageCreationDTO imageDto)
        {
            string extension = Path.GetExtension(imageDto.formFile.FileName);

            return new ImageEntity()
            {
                Name = imageDto.Name,
                Extension = extension,
                SizeInBytes = (uint)imageDto.formFile.Length
            };
        }
    }
}
