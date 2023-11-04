using LightApi.Domain.Entities;
using LightApi.EFCore.Config;
using LightApi.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace LightApi.Domain.Entities
{
    [Index(nameof(OrderNo),nameof(IsDeleted),nameof(DeletedAt), IsUnique = true)]
    public class ProduceOrder : EfEntity<int>,ISoftDelete
    {
        public string OrderNo { get; set; }

        public bool IsDeleted { get; set; }
        
        public DateTime? DeletedAt { get; set; }
    }
}
public class ProduceOrderConfig:AbstractEntityTypeConfiguration<ProduceOrder,int>
{
    
}