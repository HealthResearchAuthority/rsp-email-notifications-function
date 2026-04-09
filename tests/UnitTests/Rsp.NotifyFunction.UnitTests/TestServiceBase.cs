namespace Rsp.NotifyFunction.UnitTests;

public class TestServiceBase
{
    public TestServiceBase()
    {
        Mocker = new AutoMocker();
    }

    public AutoMocker Mocker { get; }
}