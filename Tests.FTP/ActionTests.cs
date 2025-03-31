using Apps.FTP.Actions;
using Apps.FTP.Dtos;
using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tests.FTP.Base;

namespace Tests.FTP
{
    [TestClass]
    public class ActionsTests : TestBase
    {
        private Actions _actions;

        [TestInitialize]
        public void Setup()
        {
            _actions = new Actions(InvocationContext, FileManager);
        }

        [TestMethod]
        public async Task UploadFile_ShouldUploadFile()
        {
            // Arrange
            var fileName = "test.txt";
            var filePath = "testFileUploaded.txt";
            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference() { Name = fileName},
                Path = filePath
            };

            // Act
            await _actions.UploadFile(uploadFileRequest);

            // Assert
            // Verify that the file was uploaded to the correct path
            var uploadedFile = (await _actions.ListDirectory(new ListDirectoryRequest())).DirectoriesItems.First(x => x.Name == filePath);
            Assert.IsNotNull(uploadedFile);
        }

        [TestMethod]
        public async Task DownloadFile_ShouldReturnDownloadFileResponse()
        {
            // Arrange
            var fileName = "fileToDownload.txt";
            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference() { Name = "test.txt" },
                Path = fileName
            };
            var downloadFileRequest = new DownloadFileRequest
            {
                Path = fileName
            };
            await _actions.UploadFile(uploadFileRequest);

            // Act
            var result = await _actions.DownloadFile(downloadFileRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DownloadFileResponse));
            // Verify that the file was downloaded from the correct path
            Assert.IsNotNull(result.File.Name);
        }

        [TestMethod]
        public async Task ListDirectory_ShouldReturnListDirectoryResponse()
        {
            // Arrange
            var directoryPath = "testDirectory";
            var createDirectoryRequest = new CreateDirectoryRequest
            {
                Path = directoryPath
            };
            await _actions.CreateDirectory(createDirectoryRequest);

            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference { Name = "test.txt" },
                Path = $"{directoryPath}/test.txt"
            };
            await _actions.UploadFile(uploadFileRequest);

            var listDirectoryRequest = new ListDirectoryRequest
            {
                Path = directoryPath
            };

            // Act
            var result = await _actions.ListDirectory(listDirectoryRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ListDirectoryResponse));
            Assert.AreEqual(1, result.DirectoriesItems.Count());
            // Verify that the directory listing was retrieved from the correct path
            var directoryItems = result.DirectoriesItems.ToList();
            Assert.AreEqual("test.txt", directoryItems[0].Name);
            Assert.AreEqual($"/{directoryPath}/test.txt", directoryItems[0].Path);
        }

        [TestMethod]
        public async Task DeleteFile_ShouldDeleteFile()
        {
            var fileToDelete = "fileToDelete.txt";
            // Arrange

            var uploadFileRequest = new UploadFileRequest()
            { 
                Path = fileToDelete,
                File = new FileReference() { Name = "test.txt" }
            };
            await _actions.UploadFile(uploadFileRequest);
            // Act

            await _actions.DeleteFile(fileToDelete);

            // Assert
            // Verify that the directory was deleted from the correct path
            var directories = (await _actions.ListDirectory(new ListDirectoryRequest())).DirectoriesItems.FirstOrDefault(x => x.Name == fileToDelete);
            Assert.IsNull(directories);
        }

        [TestMethod]
        public async Task RenameFile_ShouldRenameFile()
        {
            // Arrange
            var oldFilePath = "path/file1.txt";
            var newFilePath = "path/file2.txt";

            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference { Name = "test.txt" },
                Path = oldFilePath
            };
            await _actions.UploadFile(uploadFileRequest);

            var renameFileRequest = new RenameFileRequest
            {
                OldPath = oldFilePath,
                NewPath = newFilePath
            };

            // Act
            await _actions.RenameFile(renameFileRequest);

            // Assert
            // Verify that the file was renamed to the correct path
            var renamedFile = (await _actions.ListDirectory(new ListDirectoryRequest() { Path = "path"})).DirectoriesItems.First(x => x.Name == "file2.txt");
            Assert.IsNotNull(renamedFile);
        }

        [TestMethod]
        public async Task CreateDirectory_ShouldCreateDirectory()
        {
            // Arrange
            var createDirectoryRequest = new CreateDirectoryRequest
            {
                Path = "testpath"
            };

            // Act
            await _actions.CreateDirectory(createDirectoryRequest);

            // Assert
            // Verify that the directory was created at the correct path
            var directoryExists = (await _actions.ListDirectory(new ListDirectoryRequest() { Path = createDirectoryRequest.Path })) != null; 
            Assert.IsTrue(directoryExists);
        }

        [TestMethod]
        public async Task DeleteDirectory_ShouldDeleteDirectory()
        {
            var pathToDelete = "pathToDelete";
            // Arrange
            var deleteDirectoryRequest = new DeleteDirectoryRequest
            {
                Path = pathToDelete
            };

            var createDirectoryRequest = new CreateDirectoryRequest()
            { 
                Path = pathToDelete 
            };
            await _actions.CreateDirectory(createDirectoryRequest);
            // Act

            await _actions.DeleteDirectory(deleteDirectoryRequest);

            // Assert
            // Verify that the directory was deleted from the correct path
            var directories = (await _actions.ListDirectory(new ListDirectoryRequest())).DirectoriesItems.FirstOrDefault(x => x.Path == pathToDelete);
            Assert.IsNull(directories);
        }
    }
}
