using System;

namespace Gallery.API.Models
{
    public class ImageDTO
    {
        public Guid Id { get; set; }

        public Guid GalleryId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public uint SizeInBytes { get; set; }

        public ImageDTO(Guid id, Guid galleryId, string name, string extension, uint sizeInBytes)
        {
            Id = id;
            GalleryId = galleryId;
            Name = name;
            Extension = extension;
            SizeInBytes = sizeInBytes;
        }
    }
}
