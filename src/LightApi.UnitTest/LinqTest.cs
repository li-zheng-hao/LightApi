using LightApi.Infra.Extension;
using LightApi.Infra.InfraException;
using LightApi.Infra.LinqExtension;

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