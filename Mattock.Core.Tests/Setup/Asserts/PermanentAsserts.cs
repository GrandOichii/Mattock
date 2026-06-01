namespace Mattock.Core.Tests.Setup.Asserts;

public class PermanentAsserts(Permanent permanent)
{
    public PermanentAsserts IsLand()
    {
        permanent.IsLand().ShouldBeTrue();
        return this;
    }

    public PermanentAsserts ControlledBy(int playerIdx)
    {
        permanent.IsControlledBy(playerIdx).ShouldBeTrue();
        return this;
    }

    public PermanentAsserts IsUntapped()
    {
        permanent.IsUntapped().ShouldBeTrue();
        return this;
    }

    public PermanentAsserts IsTapped()
    {
        permanent.IsTapped().ShouldBeTrue();
        return this;
    }
}