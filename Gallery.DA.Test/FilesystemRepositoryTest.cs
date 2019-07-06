using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Gallery.TestUtils;

namespace Gallery.DA.Text
{
    [TestClass]
    public class FilesystemRepositoryTest
    {
        #region Init & Clean
        public FilesystemRepository filesystemRepository { get; private set; }
        private readonly ImageUtils UnitTestImageUtils = new ImageUtils();

        public FilesystemRepositoryTest()
        {
            // Create test folder and test images
            UnitTestImageUtils.CreateTestImages("./testFolder", 3);

            // Create repository
            filesystemRepository = new FilesystemRepository(UnitTestImageUtils.imagesFolder);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            //foreach (string imageName in imageNames)
            //{
            //    string imgPath = Path.Combine(UnitTestImageUtils.imagesFolder, imageName);
            //    File.Delete(imgPath);
            //}
            //Directory.Delete(UnitTestImageUtils.imagesFolder);
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

        #region RetrieveImages()
        [TestMethod]
        public void RetrieveImages()
        {
            // Arrange

            // Act
            IList<byte[]> imagesBytes = filesystemRepository.RetrieveImages().ToList();

            //Assert
            Assert.IsNotNull(imagesBytes);
            // Assert we return the same number of test images that we created for this test
            Assert.AreEqual(UnitTestImageUtils.imageNames.Count, imagesBytes.Count);
        }
        #endregion

        #region RetrieveImage()
        [TestMethod]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            byte[] imageBytes = filesystemRepository.RetrieveImage(UnitTestImageUtils.imageNames[0]);

            //Assert
            Assert.IsNotNull(imageBytes);
            Assert.IsTrue(imageBytes.Length > 0);
        }

        [TestMethod]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            byte[] imageBytes = filesystemRepository.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.IsNull(imageBytes);
        }
        #endregion
    }
}
