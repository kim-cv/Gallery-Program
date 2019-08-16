using System;

namespace Gallery.API.Models
{
    public class ImageDTO
    {
        public Guid Id { get; set; }

        public Guid galleryId { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public uint SizeInBytes { get; set; }
    }
}
