using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.FTP;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.FileManagementAndStorage];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
