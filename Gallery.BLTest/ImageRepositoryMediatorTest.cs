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

        #region RetrieveImage()
        [TestMethod]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            IImageInformation imageInformation = imageRepositoryMediator.RetrieveImage(UnitTestImageUtils.imageNames[0]);

            //Assert
            Assert.IsNotNull(imageInformation);
        }

        [TestMethod]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            IImageInformation imageInformation = imageRepositoryMediator.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.IsNull(imageInformation);
        }
        #endregion
    }
}