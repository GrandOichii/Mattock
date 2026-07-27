using System.Text;

namespace Mattock.Core.Tests.Setup;

public static class CoreLoader
{
    public static string[] Load(string dir)
    {
        return [.. Directory.GetFiles(dir).Select(File.ReadAllText)];
    }
}