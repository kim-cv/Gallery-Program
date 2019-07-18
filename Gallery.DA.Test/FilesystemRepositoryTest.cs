using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gallery.TestUtils;

namespace Gallery.DA.Text
{
    [TestClass]
    public class FilesystemRepositoryTest
    {
        #region Init & Clean
        public static FilesystemRepository filesystemRepository { get; private set; }
        private static readonly ImageUtils UnitTestImageUtils = new ImageUtils();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create test folder and test images
            UnitTestImageUtils.CreateTestImages("./testFolder", 3);

            // Create repository
            filesystemRepository = new FilesystemRepository(UnitTestImageUtils.imagesFolder);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            UnitTestImageUtils.CleanUp();
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
            FileInfo[] fileInfos = filesystemRepository.RetrieveImages();

            //Assert
            Assert.IsNotNull(fileInfos);
            // Assert we return the same number of test images that we created for this test
            Assert.AreEqual(UnitTestImageUtils.imageNames.Count, fileInfos.Length);
        }
        #endregion

        #region RetrieveImage()
        [TestMethod]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            FileInfo fileInfo = filesystemRepository.RetrieveImage(UnitTestImageUtils.imageNames[0]);

            //Assert
            Assert.IsNotNull(fileInfo);
        }

        [TestMethod]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            FileInfo fileInfo = filesystemRepository.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.IsNull(fileInfo);
        }
        #endregion
    }
}
