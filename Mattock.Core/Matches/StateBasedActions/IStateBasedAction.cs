namespace Mattock.Core.Matches.StateBasedActions;

public interface IStateBasedAction
{
    bool Apply(Match match);
}