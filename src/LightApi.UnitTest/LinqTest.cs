using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using LightApi.Infra.Extension;
using LightApi.Infra.Extension.DynamicQuery;
using LightApi.Infra.InfraException;
using LightApi.Infra.LinqExtension;
using Newtonsoft.Json;

namespace LightApi.UnitTest;

public class LinqTest
{
    
    [Theory]
    [InlineData("Contains","A","a",1)]
    [InlineData("Contains","A2","a",1)]
    [InlineData("Contains","InternalModel.A2","a",1)]
    [InlineData("Equal","A","abc",1)]
    [InlineData("Equal","A","ghsdasc",0)]
    [InlineData("Equal","A2","abc",1)]
    [InlineData("Equal","A2",null,1)]
    [InlineData("Equal","B2",1.11,1)]
    [InlineData("Equal","B2","1.11",1)]
    [InlineData("Equal","B2",null,1)]
    [InlineData("Equal","C",2,1)]
    [InlineData("Equal","C2",2,1)]
    [InlineData("Equal","C2",null,1)]
    [InlineData("Equal","D","1001-01-01",1)]
    [InlineData("Equal","D2","1001-01-01",1)]
    [InlineData("Equal","D2",null,1)]
    [InlineData("Equal","F","A",1)]
    [InlineData("Equal","F2",null,1)]
    [InlineData("NotEqual","A","abc",2)]
    [InlineData("NotEqual","A2",null,2)]
    [InlineData("NotEqual","B",1.11,2)]
    [InlineData("NotEqual","B2",null,2)]
    [InlineData("NotEqual","D","1001-01-01",2)]
    [InlineData("NotEqual","D2",null,2)]
    [InlineData("NotEqual","InternalModel.D2",null,2)]
    [InlineData("GreaterThan","B","1",3)]
    [InlineData("GreaterThan","B2","0",2)]
    [InlineData("GreaterThan","D","0001-01-01",3)]
    [InlineData("GreaterThan","D2","0001-01-01",2)]
    [InlineData("GreaterThanOrEqual","B","1.11",3)]
    [InlineData("GreaterThanOrEqual","B","1.12",2)]
    [InlineData("GreaterThanOrEqual","B2","0",2)]
    [InlineData("GreaterThanOrEqual","D","0001-01-01",3)]
    [InlineData("GreaterThanOrEqual","D2","0001-01-01",2)]
    [InlineData("GreaterThanOrEqual","D","1001-01-01",3)]
    [InlineData("LessThan","B","1.12",1)]
    [InlineData("LessThan","B2","2",1)]
    [InlineData("LessThan","InternalModel.B",2,1)]
    [InlineData("LessThanOrEqual","B","1.11",1)]
    [InlineData("LessThanOrEqual","B","1.1",0)]
    [InlineData("LessThanOrEqual","InternalModel.B",1.11,1)]
    [InlineData("IsNull","A",null,0)]
    [InlineData("IsNull","B2",null,1)]
    [InlineData("IsNull","D2",null,1)]
    [InlineData("IsNull","InternalModel.A",null,0)]
    [InlineData("IsNotNull","A",null,3)]
    [InlineData("IsNotNull","B2",null,2)]
    [InlineData("IsNotNull","D2",null,2)]
    [InlineData("IsNotNull","InternalModel.A",null,3)]
    [InlineData("StartWith","InternalModel.A","a",1)]
    [InlineData("StartWith","A","a",1)]
    [InlineData("StartWith","A","ggsdcxzcx",0)]
    [InlineData("In","A","[\"abc\"]",1)]
    [InlineData("In","F","[\"A\"]",1)]
    public void DynamicWhere(string opType,string property,object? value,int resultCount)
    {

        string queryJsonBody = string.Empty;

        if (value is string valueStr)
        {
            if(valueStr.StartsWith("["))
                queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":"+valueStr+"}";
            else
                queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":\""+value+"\"}";
        }
        else if (value ==null)
        {
            queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":null}";
        }
        else
        {
            queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":"+value+"}";
        }
        var query = JsonConvert.DeserializeObject<DynamicQueryDto>(queryJsonBody);
        var data = GetTestData();
        var result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Equal(resultCount,result.Count);
    }

    
    [Theory]
    [InlineData("Contains","A","a",1)]
    [InlineData("Contains","A2","a",1)]
    [InlineData("Contains","InternalModel.A2","a",1)]
    [InlineData("Equal","A","abc",1)]
    [InlineData("Equal","A","ghsdasc",0)]
    [InlineData("Equal","A2","abc",1)]
    [InlineData("Equal","A2",null,1)]
    [InlineData("Equal","B2",1.11,1)]
    [InlineData("Equal","B2","1.11",1)]
    [InlineData("Equal","B2",null,1)]
    [InlineData("Equal","C",2,1)]
    [InlineData("Equal","C2",2,1)]
    [InlineData("Equal","C2",null,1)]
    [InlineData("Equal","D","1001-01-01",1)]
    [InlineData("Equal","D2","1001-01-01",1)]
    [InlineData("Equal","D2",null,1)]
    [InlineData("Equal","F","A",1)]
    [InlineData("Equal","F2",null,1)]
    [InlineData("NotEqual","A","abc",2)]
    [InlineData("NotEqual","A2",null,2)]
    [InlineData("NotEqual","B",1.11,2)]
    [InlineData("NotEqual","B2",null,2)]
    [InlineData("NotEqual","D","1001-01-01",2)]
    [InlineData("NotEqual","D2",null,2)]
    [InlineData("NotEqual","InternalModel.D2",null,2)]
    [InlineData("GreaterThan","B","1",3)]
    [InlineData("GreaterThan","B2","0",2)]
    [InlineData("GreaterThan","D","0001-01-01",3)]
    [InlineData("GreaterThan","D2","0001-01-01",2)]
    [InlineData("GreaterThanOrEqual","B","1.11",3)]
    [InlineData("GreaterThanOrEqual","B","1.12",2)]
    [InlineData("GreaterThanOrEqual","B2","0",2)]
    [InlineData("GreaterThanOrEqual","D","0001-01-01",3)]
    [InlineData("GreaterThanOrEqual","D2","0001-01-01",2)]
    [InlineData("GreaterThanOrEqual","D","1001-01-01",3)]
    [InlineData("LessThan","B","1.12",1)]
    [InlineData("LessThan","B2","2",1)]
    [InlineData("LessThan","InternalModel.B",2,1)]
    [InlineData("LessThanOrEqual","B","1.11",1)]
    [InlineData("LessThanOrEqual","B","1.1",0)]
    [InlineData("LessThanOrEqual","InternalModel.B",1.11,1)]
    [InlineData("IsNull","A",null,0)]
    [InlineData("IsNull","B2",null,1)]
    [InlineData("IsNull","D2",null,1)]
    [InlineData("IsNull","InternalModel.A",null,0)]
    [InlineData("IsNotNull","A",null,3)]
    [InlineData("IsNotNull","B2",null,2)]
    [InlineData("IsNotNull","D2",null,2)]
    [InlineData("IsNotNull","InternalModel.A",null,3)]
    [InlineData("StartWith","InternalModel.A","a",1)]
    [InlineData("StartWith","A","a",1)]
    [InlineData("StartWith","A","ggsdcxzcx",0)]
    [InlineData("In","A","[\"abc\"]",1)]
    [InlineData("In","F","[\"A\"]",1)]
    public void DynamicWhereForSystemTextJson(string opType,string property,object? value,int resultCount)
    {

        string queryJsonBody = string.Empty;

        if (value is string valueStr)
        {
            if(valueStr.StartsWith("["))
                queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":"+valueStr+"}";
            else
                queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":\""+value+"\"}";
        }
        else if (value ==null)
        {
            queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":null}";
        }
        else
        {
            queryJsonBody = "{\"Property\":\""+property+"\",\"OpType\":\""+opType+"\",\"Value\":"+value+"}";
        }

        var serializerOptions = new JsonSerializerOptions()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString ,
            Converters = { new JsonStringEnumConverter() }
        };
        var query =System.Text.Json.JsonSerializer.Deserialize<DynamicQueryDto>(queryJsonBody,serializerOptions);
        var data = GetTestData();
        var result = data.AsQueryable().DynamicWhere(query!).ToList();
        Assert.Equal(resultCount,result.Count);
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