using LightApi.Infra.Helper;
using LightApi.Infra.Http;
using LightApi.Infra.InfraException;
using Microsoft.Extensions.DependencyInjection;

namespace LightApi.UnitTest;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        using var excelStream = File.OpenRead("Files/test_excel.xlsx");
        var data = ExcelHelper.GetValues(excelStream);
        Assert.True(ExcelHelper.ValidateHeaders(data, "a", "b", "c"));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfNotPositiveInt(data, 1));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfNotDouble(data, 3));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfEmptyColumn(data, 5));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfNotPositiveDouble(data, 1));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfNotInt(data, 4));
        Assert.Throws<BusinessException>(() => ExcelHelper.ThrowIfNotInRange(data, 3, "num2"));
        ExcelHelper.ThrowIfNotInRange(data, 3, "num");
        ExcelHelper.ThrowIfNotDouble(data, 4);
    }

    [Fact]
    public void UserContextTest()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddUserContextSetup<TestUserContext>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var testUserContext = serviceProvider.GetService<TestUserContext>();
        testUserContext!.UserName = "testname";
        var iuser = serviceProvider.GetService<IUser>();
        Assert.Equal("testname", iuser!.UserName);
    }

    class TestUserContext : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
    }
}
