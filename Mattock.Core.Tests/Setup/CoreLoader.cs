using System.Text;

namespace Mattock.Core.Tests.Setup;

public static class CoreLoader
{
    public static string Load(string dir)
    {
        StringBuilder coreBuilder = new();
        foreach (var file in Directory.GetFiles(dir))
        {
            coreBuilder.Append(File.ReadAllText(file));
        }
        return coreBuilder.ToString();
    }
}