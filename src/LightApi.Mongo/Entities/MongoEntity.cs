using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;
using Newtonsoft.Json;

namespace LightApi.Mongo.Entities
{
    /// <summary>
    /// Mongo数据库集合基类
    /// </summary>
    public class MongoEntity : IEntity
    {
        /// <summary>
        /// 未映射上的字段全都记录在此对象内
        /// </summary>
        [JsonIgnore]
        [BsonExtraElements]
        public BsonDocument CatchAll { get; set; }

        public string GenerateNewID()
        {
            return ObjectId.GenerateNewId().ToString();
        }
        [JsonProperty("Id")]
        [BsonId, ObjectId]
        public string ID { get; set; }

        [JsonIgnore]
        [Ignore]
        public bool _dummyProp { get; set; } //use this to specify an exclusion projection
    }
}