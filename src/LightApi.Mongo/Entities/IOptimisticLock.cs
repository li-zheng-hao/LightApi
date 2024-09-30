using System;
using MongoDB.Entities;

namespace LightApi.Mongo.Entities;

public interface IOptimisticLock : IEntity
{
    /// <summary>
    /// 版本号
    /// </summary>
    /// <value></value>
    string Version { get; set; }
}
