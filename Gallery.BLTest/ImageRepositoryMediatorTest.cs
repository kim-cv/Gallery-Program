using Gallery.BL;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gallery.Core.Interfaces;
using Gallery.TestUtils;
using Gallery.DA;
using System.Linq;

namespace Gallery.BLTest
{
    [TestClass]
    public class ImageRepositoryMediatorTest
    {
        #region Init & Clean
        public static ImageRepositoryMediator imageRepositoryMediator { get; private set; }
        private static readonly ImageUtils UnitTestImageUtils = new ImageUtils();

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create test folder and test images
            UnitTestImageUtils.CreateTestImages("./testFolder", 3);

            IImageFileRepository filesystemRepository = new FilesystemRepository(UnitTestImageUtils.imagesFolder);
            imageRepositoryMediator = new ImageRepositoryMediator(filesystemRepository);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            UnitTestImageUtils.CleanUp();
        }
        #endregion

        #region RetrieveImages()
        [TestMethod]
        public void RetrieveImages()
        {
            // Arrange

            // Act
            IEnumerable<IImageInformation> imageInformations = imageRepositoryMediator.RetrieveImages(0, 3);

            //Assert
            Assert.IsNotNull(imageInformations);
            // Assert we return the same number of test images that we created for this test
            Assert.AreEqual(UnitTestImageUtils.imageNames.Count, imageInformations.Count());
        }
        #endregion

        #region NextImage()
        [TestMethod]
        public void NextImage()
        {
            // Arrange
            IImageInformation[] imageInformations = imageRepositoryMediator.RetrieveImages(0, 3).ToArray();
            imageRepositoryMediator.CurrentLargeImage = imageInformations[1];

            // Act
            IImageInformation nextImage = imageRepositoryMediator.NextImage();

            //Assert
            Assert.IsNotNull(nextImage);
            Assert.AreEqual(nextImage.fileInfo, imageInformations[2].fileInfo);
        }
        [TestMethod]
        public void NextImage_null()
        {
            // Arrange
            IImageInformation[] imageInformations = imageRepositoryMediator.RetrieveImages(0, 3).ToArray();
            imageRepositoryMediator.CurrentLargeImage = imageInformations[2];

            // Act
            IImageInformation nullImage = imageRepositoryMediator.NextImage();

            //Assert
            Assert.IsNull(nullImage);
        }
        #endregion

        #region PreviousImage()
        [TestMethod]
        public void PreviousImage()
        {
            // Arrange
            IImageInformation[] imageInformations = imageRepositoryMediator.RetrieveImages(0, 3).ToArray();
            imageRepositoryMediator.CurrentLargeImage = imageInformations[1];

            // Act
            IImageInformation previousImage = imageRepositoryMediator.PreviousImage();

            //Assert
            Assert.IsNotNull(previousImage);
            Assert.AreEqual(previousImage.fileInfo, imageInformations[0].fileInfo);
        }
        [TestMethod]
        public void PreviousImage_null()
        {
            // Arrange
            IImageInformation[] imageInformations = imageRepositoryMediator.RetrieveImages(0, 3).ToArray();
            imageRepositoryMediator.CurrentLargeImage = imageInformations[0];

            // Act
            IImageInformation nullImage = imageRepositoryMediator.PreviousImage();

            //Assert
            Assert.IsNull(nullImage);
        }
        #endregion
    }
}