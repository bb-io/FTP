using Apps.FTP.Actions;
using Apps.FTP.Models.Requests;
using Tests.FTP.Base;

namespace Tests.FTP
{
    [TestClass]
    public class FolderTests : TestBase
    {
        private FolderActions _actions;
        private FileActions _fileActions;

        [TestInitialize]
        public void Setup()
        {
            _actions = new FolderActions(InvocationContext, FileManager);
            _fileActions = new FileActions(InvocationContext, FileManager);
        }

        [TestMethod]
        public async Task CreateDirectory_ShouldCreateDirectory()
        {
            // Arrange
            var createDirectoryRequest = new CreateDirectoryRequest
            {
                ParentFolderId = "testpath"
            };

            // Act
            await _actions.CreateDirectory(createDirectoryRequest);

            // Assert
            // Verify that the directory was created at the correct path
            var directoryExists = (await _fileActions.ListDirectory(new ListDirectoryRequest() { FolderId = createDirectoryRequest.ParentFolderId })) != null;
            Assert.IsTrue(directoryExists);
        }

        [TestMethod]
        public async Task DeleteDirectory_ShouldDeleteDirectory()
        {
            var pathToDelete = "/test/test.txt";
            // Arrange
            var deleteDirectoryRequest = new DeleteDirectoryRequest
            {
                FolderId = pathToDelete
            };

            var createDirectoryRequest = new CreateDirectoryRequest()
            {
                ParentFolderId = pathToDelete
            };
            await _actions.CreateDirectory(createDirectoryRequest);
            // Act

            await _actions.DeleteDirectory(deleteDirectoryRequest);

            // Assert
            // Verify that the directory was deleted from the correct path
            var directories = (await _fileActions.ListDirectory(new ListDirectoryRequest())).Files.FirstOrDefault(x => x.Path == pathToDelete);
            Assert.IsNull(directories);
        }
    }
}
