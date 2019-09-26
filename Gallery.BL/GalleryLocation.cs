namespace Gallery.BL
{
    public class GalleryLocation
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public GalleryLocation(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}