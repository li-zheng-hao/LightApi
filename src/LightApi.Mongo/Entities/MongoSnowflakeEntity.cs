using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Newtonsoft.Json;
using Yitter.IdGenerator;

namespace LightApi.Mongo.Entities
{
    /// <summary>
    /// Mongo数据库集合基类 雪花Id作为主键
    /// </summary>
    public class MongoSnowflakeEntity : IEntity
    {
        /// <summary>
        /// 未映射上的字段全都记录在此对象内
        /// </summary>
        [JsonIgnore]
        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }

        public bool HasDefaultID()
        {
            return Id == 0;
        }

        [BsonId]
        public long Id { get; set; }

        object IEntity.GenerateNewID()
        {
            return YitIdHelper.NextId();
        }
    }
}