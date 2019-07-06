using Gallery.BL;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace Gallery.BLTest
{
    /*
    [TestClass]
    public class FilesystemRepositoryTest
    {
        #region Init & Clean
        public FilesystemRepository filesystemRepository { get; private set; }
        public readonly string imagesFolder = "./testFolder";
        public readonly IList<string> imageNames = new List<string>();

        public FilesystemRepositoryTest()
        {
            // Create test folder
            Directory.CreateDirectory(imagesFolder);

            // Create test images
            for (int i = 0; i < 3; i++)
            {
                string imageName = "testImage" + i.ToString() + ".jpg";
                imageNames.Add(imageName);

                // Create test image
                byte[] imgBytes = CreateGridImage(10, 10, 0, i, 30);

                // Save test image
                string imgPath = Path.Combine(imagesFolder, imageName);
                using (FileStream fileStream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(imgBytes, 0, imgBytes.Length);
                }
            }

            // Create repository
            filesystemRepository = new FilesystemRepository(imagesFolder);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            //foreach (string imageName in imageNames)
            //{
            //    string imgPath = Path.Combine(imagesFolder, imageName);
            //    File.Delete(imgPath);
            //}
            //Directory.Delete(imagesFolder);
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
        #endregion  

        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public void Ctor_invalid_directory_path()
        {
            // Arrange

            // Act
            new FilesystemRepository("./unknownFolder");

            //Assert
        }

        #region RetrieveImagesAsThumbs()
        [TestMethod]
        public void RetrieveImagesAsThumbs()
        {
            // Arrange

            // Act
            IList<BitmapSource> images = filesystemRepository.RetrieveImagesAsThumbs();

            //Assert
            Assert.IsNotNull(images);
            Assert.AreEqual(imageNames.Count, images.Count);
        }
        #endregion

        #region RetrieveImage()
        [TestMethod]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            BitmapSource image = filesystemRepository.RetrieveImage(imageNames[0]);

            //Assert
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            BitmapSource image = filesystemRepository.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.IsNull(image);
        }
        #endregion
    }
    */
}
