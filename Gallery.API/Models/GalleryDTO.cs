using System;

namespace Gallery.API.Models
{
    public class GalleryDTO
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public int NumberOfImages { get; set; }

        public GalleryDTO(Guid id, Guid ownerId, string name, int numberOfImages)
        {
            Id = id;
            OwnerId = ownerId;
            Name = name;
            NumberOfImages = numberOfImages;
        }
    }
}
