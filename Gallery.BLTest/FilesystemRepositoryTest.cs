using Gallery.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Xunit;

namespace Gallery.BLTest
{
    public class FilesystemRepositoryFixture : IDisposable
    {
        public FilesystemRepository provider { get; private set; }
        public readonly string imagesFolder = "./testFolder";
        public readonly string imageName = "testImage.jpg";

        public FilesystemRepositoryFixture()
        {
            // Create test folder
            Directory.CreateDirectory(imagesFolder);

            // Create test image
            byte[] imgBytes = CreateGridImage(10, 10, 9, 9, 30);

            // Save test image
            string imgPath = Path.Combine(imagesFolder, imageName);
            using (FileStream fileStream = new FileStream(imgPath, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(imgBytes);
            }

            // Create provider
            provider = new FilesystemRepository(imagesFolder);
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
                    Rectangle rect = new Rectangle(boxSize * (cellXPosition - 1), boxSize * (cellYPosition - 1), boxSize, boxSize);
                    g.FillRectangle(new SolidBrush(Color.Red), rect);

                    //Draw cross
                    g.DrawLine(pen, boxSize * (cellXPosition - 1), boxSize * (cellYPosition - 1), boxSize * cellXPosition, boxSize * cellYPosition);
                    g.DrawLine(pen, boxSize * (cellXPosition - 1), boxSize * cellYPosition, boxSize * cellXPosition, boxSize * (cellYPosition - 1));

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

        public void Dispose()
        {
            string imgPath = Path.Combine(imagesFolder, imageName);
            File.Delete(imgPath);
            Directory.Delete(imagesFolder);
        }
    }

    public class FilesystemRepositoryTest : IClassFixture<FilesystemRepositoryFixture>
    {
        FilesystemRepositoryFixture fixture;

        public FilesystemRepositoryTest(FilesystemRepositoryFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Ctor_invalid_directory_path()
        {
            // Arrange

            // Act

            //Assert
            Assert.Throws<DirectoryNotFoundException>(() => new FilesystemRepository("./unknownFolder"));
        }

        #region RetrieveImagesAsThumbs()
        [Fact]
        public void RetrieveImagesAsThumbs()
        {
            // Arrange

            // Act
            IList<Image> images = fixture.provider.RetrieveImagesAsThumbs();

            //Assert
            Assert.NotNull(images);
            Assert.Equal(1, images.Count);
            Assert.NotNull(images[0]);
        }
        #endregion

        #region RetrieveImage()
        [Fact]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            Image image = fixture.provider.RetrieveImage(fixture.imageName);

            //Assert
            Assert.NotNull(image);
        }

        [Fact]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            Image image = fixture.provider.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.Null(image);
        }
        #endregion
    }
}
