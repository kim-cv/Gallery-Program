using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Gallery.API.Models;
using Gallery.API.Repositories;


namespace Gallery.API.Test
{
    [TestClass]
    public class FileSystemRepositoryTest
    {
        private static string ContentRootFolderName = "UnknownContentRootPath";
        private static string UnknownUploadFolderImagesFolderName = "UnknownUploadFolderImagesPath";
        private static string RandomFileName = "RandomFileName";
        private static string RandomFileExtension = ".txt";
        private static string FileData = "This is unittest text";
        private static string FolderPath = Path.Combine(ContentRootFolderName, UnknownUploadFolderImagesFolderName);
        private static string FilePath = Path.Combine(FolderPath, RandomFileName);
        private static string FilePathWithExtension = Path.ChangeExtension(FilePath, RandomFileExtension);
        private static Mock<IWebHostEnvironment> WebHostEnvironment;
        private static IOptions<ContentFolders> FolderOptions;

        [ClassInitialize]
        public static void InitTestClass(TestContext testContext)
        {
            // Mock IWebHostEnvironment & Options
            WebHostEnvironment = new Mock<IWebHostEnvironment>();
            WebHostEnvironment.SetupProperty(x => x.ContentRootPath, ContentRootFolderName);
            FolderOptions = Options.Create(new ContentFolders() { UploadFolderImages = UnknownUploadFolderImagesFolderName });

            if (Directory.Exists(FolderPath))
            {
                File.Delete(FilePathWithExtension);
                Directory.Delete(FolderPath);
                Directory.Delete(ContentRootFolderName);
            }
        }

        #region RetrieveFile
        [TestMethod]
        public async Task RetrieveFile()
        {
            // Arrange            
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);

            // Create test folder and test file
            Directory.CreateDirectory(FolderPath);
            using (var stream = File.CreateText(FilePathWithExtension))
            {
                stream.Write(FileData);
                stream.Flush();
            }

            // Act
            byte[] result = await fileSystemRepository.RetrieveFile(RandomFileName, RandomFileExtension);

            // Cleanup test folder and test file
            File.Delete(FilePathWithExtension);
            Directory.Delete(FolderPath);
            Directory.Delete(ContentRootFolderName);

            // Assert
            Assert.IsTrue(result.Length > 0);
            string returnedData = Encoding.UTF8.GetString(result);
            Assert.AreEqual(FileData, returnedData);
        }

        /*
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public async Task RetrieveFile_folder_not_exist()
        {
            // Arrange
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);

            // Act
            await fileSystemRepository.RetrieveFile(RandomFileName, RandomFileExtension);

            // Assert
            // Expecting exception
        }
        */

        [TestMethod]
        public async Task RetrieveFile_file_not_exist()
        {
            // Arrange
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);

            // Create test folder
            Directory.CreateDirectory(FolderPath);

            // Act
            try
            {
                await fileSystemRepository.RetrieveFile(RandomFileName, RandomFileExtension);
            }
            catch (FileNotFoundException ex)
            {
                // Expecting exception
                // Cleanup
                Directory.Delete(FolderPath);
                Directory.Delete(ContentRootFolderName);
                return;
            }

            Assert.Fail("Expected to throw Exception: FileNotFoundException");
        }
        #endregion

        #region SaveFile
        [TestMethod]
        public async Task SaveFile()
        {
            // Arrange            
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);
            var fileByteArray = Encoding.UTF8.GetBytes(FileData);

            // Create test folder
            Directory.CreateDirectory(FolderPath);

            // Act
            await fileSystemRepository.SaveFile(fileByteArray, RandomFileName, RandomFileExtension);

            // Cleanup test folder and test file
            File.Delete(FilePathWithExtension);
            Directory.Delete(FolderPath);
            Directory.Delete(ContentRootFolderName);

            // Assert
        }
        
        /*
        [TestMethod]
        [ExpectedException(typeof(DirectoryNotFoundException))]
        public async Task SaveFile_folder_not_exist()
        {
            // Arrange
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);
            var fileByteArray = Encoding.UTF8.GetBytes(FileData);

            // Act
            await fileSystemRepository.SaveFile(fileByteArray, RandomFileName, RandomFileExtension);

            // Assert
            // Expecting exception
        }
        */
        #endregion

        #region DeleteFile
        [TestMethod]
        public async Task DeleteFile()
        {
            // Arrange            
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);

            // Create test folder and test file
            Directory.CreateDirectory(FolderPath);
            using (var stream = File.CreateText(FilePathWithExtension))
            {
                stream.Write(FileData);
                stream.Flush();
            }

            // Act
            fileSystemRepository.DeleteFile(RandomFileName, RandomFileExtension);

            // Assert
            try
            {
                await fileSystemRepository.RetrieveFile(RandomFileName, RandomFileExtension);
            }
            catch (FileNotFoundException ex)
            {
                // Expecting exception
                // Cleanup
                Directory.Delete(FolderPath);
                Directory.Delete(ContentRootFolderName);
                return;
            }

            Assert.Fail("Expected to throw Exception: FileNotFoundException");
        }

        [TestMethod]
        public void DeleteFile_folder_not_exist()
        {
            // Arrange
            var fileSystemRepository = new FileSystemRepository(WebHostEnvironment.Object, FolderOptions);

            // Act
            fileSystemRepository.DeleteFile(RandomFileName, RandomFileExtension);

            // Assert
        }
        #endregion
    }
}
