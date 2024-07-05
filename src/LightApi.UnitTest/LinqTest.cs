using System.Reflection;
using LightApi.Infra.Extension;
using LightApi.Infra.Extension.DynamicQuery;
using LightApi.Infra.InfraException;
using LightApi.Infra.LinqExtension;
using Newtonsoft.Json;

namespace LightApi.UnitTest;

public class LinqTest
{
    [Fact]
    public void CheckEx()
    {
        int? a = null;
        Assert.Throws<BusinessException>(()=>a.NotNullOrEmptyEx());
    }
    [Fact]
    public void TestLinq()
    {
        var list = new List<Model>()
        {
            new Model()
            {
                PropA = "A",
                PropB = 1,
                SubModel = new SubModel()
                {
                    SubPropA = "SubA"
                }
            },
            new Model()
            {
                PropA = "B",
                PropB = 2,
                SubModel = new SubModel()
                {
                    SubPropA = "SubB"
                }
            },
            
        };

        var sortedList=list.AsQueryable().DynamicOrder("propA", true).ToList();
        Assert.Equal("B",sortedList.First().PropA);
        
        var sortedList2=list.AsQueryable().DynamicOrder("subModel.subPropA", true).ToList();
        Assert.Equal("SubB",sortedList2.First().SubModel.SubPropA);
    }


    [Fact]
    public void DynamicWhere_Contains()
    {
        string queryJsonBody = "{\"Property\":\"A\",\"OpType\":\"Contains\",\"Value\":\"a\"}";
        var query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        var data = GetTestData();

        var result = data.AsQueryable().DynamicWhere(query!).ToList();

        Assert.Single(result);

        queryJsonBody = "{\"Property\":\"A2\",\"OpType\":\"Contains\",\"Value\":\"a\"}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"InternalModel.A2\",\"OpType\":\"Contains\",\"Value\":\"a\"}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
    }
    [Fact]
    public void DynamicWhere_Equal()
    {
        string queryJsonBody = "{\"Property\":\"A\",\"OpType\":\"Equal\",\"Value\":\"abc\"}";
        var query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        var data = GetTestData();

        var result = data.AsQueryable().DynamicWhere(query!).ToList();

        Assert.Single(result);

        queryJsonBody = "{\"Property\":\"A\",\"OpType\":\"Equal\",\"Value\":\"ghsdasc\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Empty(result);
        
        
        queryJsonBody = "{\"Property\":\"A2\",\"OpType\":\"Equal\",\"Value\":\"abc\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"A2\",\"OpType\":\"Equal\",\"Value\":null}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"B\",\"OpType\":\"Equal\",\"Value\":1.11}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"B\",\"OpType\":\"Equal\",\"Value\":\"1.11\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        
        queryJsonBody = "{\"Property\":\"B2\",\"OpType\":\"Equal\",\"Value\":null}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"C\",\"OpType\":\"Equal\",\"Value\":2}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"C\",\"OpType\":\"Equal\",\"Value\":null}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        Assert.Throws<BusinessException>(() =>
        {
            data.AsQueryable().DynamicWhere(query!).ToList();
        });

        queryJsonBody = "{\"Property\":\"C2\",\"OpType\":\"Equal\",\"Value\":2}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"C2\",\"OpType\":\"Equal\",\"Value\":null}";

        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);

        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        
        queryJsonBody = "{\"Property\":\"D\",\"OpType\":\"Equal\",\"Value\":\"1001-01-01\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"D2\",\"OpType\":\"Equal\",\"Value\":\"1001-01-01\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"D2\",\"OpType\":\"Equal\",\"Value\":null}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"F\",\"OpType\":\"Equal\",\"Value\":\"A\"}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
        
        queryJsonBody = "{\"Property\":\"F2\",\"OpType\":\"Equal\",\"Value\":null}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Single(result);
    }

    
    [Fact]
    public void DynamicWhere_NotEqual()
    {
        string queryJsonBody = "{\"Property\":\"A\",\"OpType\":\"NotEqual\",\"Value\":\"abc\"}";
        var query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        var data = GetTestData();

        var result = data.AsQueryable().DynamicWhere(query!).ToList();

        Assert.Equal(2,result.Count);
        
        queryJsonBody = "{\"Property\":\"A2\",\"OpType\":\"NotEqual\",\"Value\":null}";
        
        query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        
        result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Equal(2,result.Count);

        
    }

    private List<Model1> GetTestData()
    {
        List<Model1> model1s = new List<Model1>();

        model1s.Add(new Model1()
        {
            A = "abc",
            B = 1.11,
            C = 2,
            D = new DateTime(1001, 1, 1),
            E = "abc",
            F = TestEnum.A,
            A2 = "abc",
            B2 = 1.11,
            C2 = 2,
            D2 = new DateTime(1001, 1, 1),
            E2 = "abc",
            F2 = TestEnum.A,

            
            InternalModel = new Model2()
            {
                A = "abc",
                B = 1.11,
                C = 2,
                D = new DateTime(1001, 1, 1),
                E = "abc",
                A2 = "abc",
                B2 = 1.11,
                C2 = 2,
                D2 = new DateTime(1001, 1, 1),
                E2 = "abc",
            }
            
        });
        model1s.Add(new Model1()
        {
            A = "def",
            B = 222.22,
            C = 222,
            D = new DateTime(3001, 1, 1),
            E = "def",
            F = TestEnum.B,
            A2 = "def",
            B2 = 222.22,
            C2 = 222,
            D2 = new DateTime(3001, 1, 1),
            E2 = "def",
            F2 = TestEnum.B,

            InternalModel = new Model2()
            {
                A = "def",
                B = 222.22,
                C = 222,
                D = new DateTime(3001, 1, 1),
                E = "def",
                A2 = "def",
                B2 = 222.22,
                C2 = 222,
                D2 = new DateTime(3001, 1, 1),
                E2 = "def",
            }
        });
        model1s.Add(new Model1()
        {
            A = "ghi",
            B = 444.44,
            C = 444,
            D = new DateTime(4001, 1, 1),
            E = "ghi",
            F = TestEnum.B,
            A2 = null,
            B2 = null,
            C2 = null,
            D2 =null,
            E2 =null,
            F2 = null,
            InternalModel = new Model2()
            {
                A = "ghi",
                B = 444.44,
                C = 444,
                D = new DateTime(4001, 1, 1),
                E = "ghi",
                A2 = null,
                B2 = null,
                C2 = null,
                D2 =null,
                E2 =null,
            }
        });
        return model1s;
    }
}

internal class Model
{
    public string PropA { get; set; }
    
    public int PropB { get; set; }
    
    public SubModel SubModel { get; set; }
}

internal class SubModel
{
    public string SubPropA { get; set; }
}

internal class Model1
{
    public string A { get; set; }
    public double B { get; set; }
    public int C { get; set; }
    public DateTime D { get; set; }
    public string E { get; set; }
    public TestEnum F { get; set; }
    public string? A2 { get; set; }
    public double? B2 { get; set; }
    public int? C2 { get; set; }
    public DateTime? D2 { get; set; }
    public string? E2 { get; set; }
    public TestEnum? F2 { get; set; }

    public Model2 InternalModel { get; set; }
    
    public Model2? InternalModel2{ get; set; }

}

internal class Model2
{
    public string A { get; set; }
    public double B { get; set; }
    public int C { get; set; }
    public DateTime D { get; set; }
    public string E { get; set; }
    
    public string? A2 { get; set; }
    public double? B2 { get; set; }
    public int? C2 { get; set; }
    public DateTime? D2 { get; set; }
    public string? E2 { get; set; }

}

internal class DynamicQueryDto
{
    public string Property { get; set; }
    public DynamicOpType OpType { get; set; }
    public object? Value { get; set; }
}

internal enum TestEnum
{
    A,
    B,
    C
}

internal static class TestExtension
{
    public static IQueryable<T> DynamicWhere<T>(this IQueryable<T> queryable, DynamicQueryDto queryDto)
    {
        return queryable.DynamicWhere(queryDto.Property, queryDto.Value, queryDto.OpType);
    }
}