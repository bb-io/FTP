using Apps.FTP.Webhooks;
using Tests.FTP.Base;

namespace Tests.FTP;

[TestClass]
public class PollingListTests : TestBase
{
    [TestMethod]
    public void TestMethod1()
    {

        var pollinglist = new PollingList(InvocationContext);
        pollinglist.OnFilesAddedOrUpdated(new Blackbird.Applications.Sdk.Common.Polling.PollingEventRequest<Apps.FTP.Webhooks.Polling.Memory.FTPMemory>(), new Apps.FTP.Webhooks.Payload.ParentFolderInput());

        Assert.Fail();
    }
}
