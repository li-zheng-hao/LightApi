using MongoDB.Entities;

namespace LightApi.Infra.FileStorage;

public class MongoStorage : FileEntity, ICreatedOn
{
    static MongoStorage()
    {
        DB.Index<MongoStorage>()
            .Key(it => it.CreatedOn, KeyType.Descending)
            .Key(it => it.IsTempFile, KeyType.Ascending)
            .CreateAsync();
    }

    /// <summary>
    /// 文件名 带扩展
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 是否为临时文件(定期删除)
    /// </summary>
    public bool IsTempFile { get; set; }

    public DateTime CreatedOn { get; set; }
}
