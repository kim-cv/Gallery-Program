using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Gallery.TestUtils
{
    public class ImageUtils
    {
        public string imagesFolder;
        public IList<string> imageNames;

        public void CreateTestImages(string testFolderPath, int numberOfImages)
        {
            CreateImagesTestFolder(testFolderPath, numberOfImages);
            CreateTestImageFiles();
        }

        private void CreateImagesTestFolder(string testFolderPath, int numberOfImages)
        {
            imageNames = new List<string>(numberOfImages);

            // Create test folder
            imagesFolder = testFolderPath;
            Directory.CreateDirectory(imagesFolder);

            // Create test image names
            for (int i = 0; i < numberOfImages; i++)
            {
                string imageName = "testImage" + i.ToString() + ".jpg";
                imageNames.Add(imageName);
            }
        }

        private void CreateTestImageFiles()
        {
            int i = 0;
            foreach (string imageName in imageNames)
            {
                // Create test image
                byte[] imgBytes = CreateGridImage(10, 10, 0, i, 30);

                // Save test image
                string imgPath = Path.Combine(imagesFolder, imageName);
                using (FileStream fileStream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(imgBytes, 0, imgBytes.Length);
                }

                i++;
            }
        }

        private byte[] CreateGridImage(
        int maxXCells,
        int maxYCells,
        int cellXPosition,
        int cellYPosition,
        int boxSize)
        {
            using (var bitmap = new Bitmap(maxXCells * boxSize + 1, maxYCells * boxSize + 1))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Yellow);
                    Pen pen = new Pen(Color.Black)
                    {
                        Width = 1
                    };

                    //Draw red rectangle to go behind cross
                    Rectangle rect = new Rectangle(boxSize * cellXPosition, boxSize * cellYPosition, boxSize, boxSize);
                    g.FillRectangle(new SolidBrush(Color.Red), rect);

                    //Draw cross
                    g.DrawLine(pen, boxSize * cellXPosition, boxSize * cellYPosition, boxSize * cellXPosition, boxSize * cellYPosition);
                    g.DrawLine(pen, boxSize * cellXPosition, boxSize * cellYPosition, boxSize * cellXPosition, boxSize * cellYPosition);

                    //Draw horizontal lines
                    for (int i = 0; i <= maxXCells; i++)
                    {
                        g.DrawLine(pen, (i * boxSize), 0, i * boxSize, boxSize * maxYCells);
                    }

                    //Draw vertical lines            
                    for (int i = 0; i <= maxYCells; i++)
                    {
                        g.DrawLine(pen, 0, (i * boxSize), boxSize * maxXCells, i * boxSize);
                    }
                }

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Jpeg);
                    return stream.ToArray();
                }
            }
        }
    }
}