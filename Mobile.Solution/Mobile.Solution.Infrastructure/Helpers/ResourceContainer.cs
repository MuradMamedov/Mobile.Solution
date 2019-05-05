using System.Resources;
using Mobile.Solution.Infrastructure.Properties;

namespace Mobile.Solution.Infrastructure.Helpers
{
    public class ResourceContainer
    {
        public static ResourceManager ResourceManager { get; } = Resources.ResourceManager;
    }
}