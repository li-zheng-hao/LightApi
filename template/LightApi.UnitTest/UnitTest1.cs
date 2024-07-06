using System.ComponentModel;

namespace LightApi.UnitTest;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Type type = typeof(DateTime?);
        var resultValue = Activator.CreateInstance(type,DateTime.Now);

        Assert.Equal(resultValue!.GetType().FullName,type.FullName);
    }
}