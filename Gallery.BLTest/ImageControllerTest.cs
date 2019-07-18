using Gallery.BL;
using Gallery.TestUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Gallery.BLTest
{
    [TestClass]
    public class ImageControllerTest
    {
        #region Init & Clean
        private static readonly ImageUtils UnitTestImageUtils = new ImageUtils();
        private static readonly string directoryUrl = "./testFolder";
        private static FileInfo[] fileInfos;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Create test folder and test images
            UnitTestImageUtils.CreateTestImages(directoryUrl, 3);

            // Check if directory exist
            if (Directory.Exists(directoryUrl) == false)
            {
                throw new DirectoryNotFoundException();
            }

            // Get all filenames in directory
            string[] filenamesWithPath = Directory.GetFiles(directoryUrl, "*.*", SearchOption.TopDirectoryOnly);

            // Filter to only get filenames with certain extension
            IList<string> allowedExtension = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
            IEnumerable<string> filteredFilenamesWithPath = filenamesWithPath
                .Where(tmpFilename => allowedExtension.Contains(Path.GetExtension(tmpFilename)));

            // Map to fileinfo
            IEnumerable<FileInfo> mappedToFileInfos = filteredFilenamesWithPath
                .Select(tmpFilenameWithPath => new FileInfo(tmpFilenameWithPath));

            // Set fileInfos
            fileInfos = mappedToFileInfos.ToArray();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            UnitTestImageUtils.CleanUp();
        }
        #endregion

        #region FileInfoToThumbnail()
        [TestMethod]
        public async Task FileInfoToThumbnail()
        {
            FileInfo fileinfo = fileInfos[0];
            BitmapSource originalBitmapSource = await ImageController.FileInfoToBitmapSource(fileinfo);
            BitmapSource thumbBitmapSource = await ImageController.FileInfoToThumbnail(fileinfo);

            // Not nulls
            Assert.IsNotNull(originalBitmapSource);
            Assert.IsNotNull(thumbBitmapSource);

            // Thumb is smaller than original
            Assert.IsTrue(thumbBitmapSource.PixelWidth < originalBitmapSource.PixelWidth);
            Assert.IsTrue(thumbBitmapSource.PixelHeight < originalBitmapSource.PixelHeight);
            // Thumb is 100px width and height
            Assert.AreEqual(thumbBitmapSource.PixelWidth, 100);
            Assert.AreEqual(thumbBitmapSource.PixelHeight, 100);
        }
        #endregion

        #region FileInfoToBitmapSource()
        [TestMethod]
        public async Task FileInfoToBitmapSource()
        {
            BitmapSource bitmapSource = await ImageController.FileInfoToBitmapSource(fileInfos[0]);

            Assert.IsNotNull(bitmapSource);
        }
        #endregion
    }
}
