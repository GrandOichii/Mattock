using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Loaders;

/// <summary>
/// Card loader
/// </summary>
public interface ICardLoader
{
    CardTemplate Load(string id);
}