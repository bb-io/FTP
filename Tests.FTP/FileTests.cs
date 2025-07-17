using Apps.FTP.Actions;
using Apps.FTP.Dtos;
using Apps.FTP.Models.Requests;
using Apps.FTP.Models.Responses;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tests.FTP.Base;

namespace Tests.FTP
{
    [TestClass]
    public class ActionsTests : TestBase
    {
        private FileActions _actions;
        private FolderActions _folderActions;

        [TestInitialize]
        public void Setup()
        {
            _actions = new FileActions(InvocationContext, FileManager);
            _folderActions = new FolderActions(InvocationContext, FileManager);
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
                FolderId = filePath
            };

            // Act
            await _actions.UploadFile(uploadFileRequest);

            // Assert
            // Verify that the file was uploaded to the correct path
            var uploadedFile = (await _actions.ListDirectory(new ListDirectoryRequest())).Files.First(x => x.Name == filePath);
            Assert.IsNotNull(uploadedFile);
        }

        [TestMethod]
        public async Task DownloadFile_ShouldReturnDownloadFileResponse()
        {
            // Arrange
            var fileName = "/testDirectory/test.txt/test.txt";
            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference() { Name = "test.txt" },
                FolderId = fileName
            };
            var downloadFileRequest = new DownloadFileRequest
            {
                FileId = fileName
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
                ParentFolderId = directoryPath
            };
            await _folderActions.CreateDirectory(createDirectoryRequest);

            var uploadFileRequest = new UploadFileRequest
            {
                File = new FileReference { Name = "test.txt" },
                FolderId = $"{directoryPath}/test.txt"
            };
            await _actions.UploadFile(uploadFileRequest);

            var listDirectoryRequest = new ListDirectoryRequest
            {
                FolderId = directoryPath
            };

            // Act
            var result = await _actions.ListDirectory(listDirectoryRequest);

            // Assert
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ListDirectoryResponse));
            Assert.AreEqual(1, result.Files.Count());
            // Verify that the directory listing was retrieved from the correct path
            var directoryItems = result.Files.ToList();
            Assert.AreEqual("test.txt", directoryItems[0].Name);
            Assert.AreEqual($"/{directoryPath}/test.txt", directoryItems[0].FileId);
        }

        [TestMethod]
        public async Task DeleteFile_ShouldDeleteFile()
        {
            var fileToDelete = new DeleteFileRequest{ FileId= "/testDirectory/test.txt/test.txt" };
            // Arrange

            var uploadFileRequest = new UploadFileRequest()
            {
                FolderId = fileToDelete.FileId,
                File = new FileReference() { Name = "test.txt" }
            };
            await _actions.UploadFile(uploadFileRequest);
            // Act

            await _actions.DeleteFile(fileToDelete);

            // Assert
            // Verify that the directory was deleted from the correct path
            var directories = (await _actions.ListDirectory(new ListDirectoryRequest())).Files.FirstOrDefault(x => x.Name == fileToDelete.FileId);
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
                FolderId = oldFilePath
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
            var renamedFile = (await _actions.ListDirectory(new ListDirectoryRequest() { FolderId = "path"})).Files.First(x => x.Name == "file2.txt");
            Assert.IsNotNull(renamedFile);
        }      
    }
}
