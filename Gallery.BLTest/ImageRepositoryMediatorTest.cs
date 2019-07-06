using Gallery.BL;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media.Imaging;
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
        public ImageRepositoryMediator imageRepositoryMediator { get; private set; }
        private readonly ImageUtils UnitTestImageUtils = new ImageUtils();

        public ImageRepositoryMediatorTest()
        {
            // Create test folder and test images
            UnitTestImageUtils.CreateTestImages("./testFolder", 3);

            IImageFileRepository filesystemRepository = new FilesystemRepository(UnitTestImageUtils.imagesFolder);
            imageRepositoryMediator = new ImageRepositoryMediator(filesystemRepository);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
        }
        #endregion

        #region RetrieveImagesAsThumbs()
        [TestMethod]
        public void RetrieveImagesAsThumbs()
        {
            // Arrange

            // Act
            IList<BitmapSource> images = imageRepositoryMediator.RetrieveImagesAsThumbs().ToList();

            //Assert
            Assert.IsNotNull(images);
            // Assert we return the same number of test images that we created for this test
            Assert.AreEqual(UnitTestImageUtils.imageNames.Count, images.Count);
        }
        #endregion

        #region RetrieveImage()
        [TestMethod]
        public void RetrieveImage()
        {
            // Arrange

            // Act
            BitmapSource image = imageRepositoryMediator.RetrieveImage(UnitTestImageUtils.imageNames[0]);

            //Assert
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void RetrieveImage_unknown_image()
        {
            // Arrange

            // Act
            BitmapSource image = imageRepositoryMediator.RetrieveImage("unknownImage.jpg");

            //Assert
            Assert.IsNull(image);
        }
        #endregion
    }
}