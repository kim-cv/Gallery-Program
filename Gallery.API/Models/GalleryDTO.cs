using System;

namespace Gallery.API.Models
{
    public class GalleryDTO
    {
        public Guid Id { get; set; }
        public Guid ownerId { get; set; }
        public string Name { get; set; }
        public int NumberOfImages { get; set; }
    }
}
