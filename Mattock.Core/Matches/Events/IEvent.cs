namespace Mattock.Core.Matches.Events;

public interface IEvent
{
    Task Do(Match match);
}