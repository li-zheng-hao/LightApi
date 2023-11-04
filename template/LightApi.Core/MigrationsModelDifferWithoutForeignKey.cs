using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace LightApi.Core;

/// <summary>
///  用于避免EFCore生成迁移脚本时，会自动创建外键的问题
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<挂起>")]
public class MigrationsModelDifferWithoutForeignKey : MigrationsModelDiffer
{
    public MigrationsModelDifferWithoutForeignKey
    (
        IRelationalTypeMappingSource typeMappingSource,
        IMigrationsAnnotationProvider migrationsAnnotationProvider,
        IRowIdentityMapFactory rowIdentityMapFactory,
        CommandBatchPreparerDependencies commandBatchPreparerDependencies)
        : base(
            typeMappingSource,
            migrationsAnnotationProvider,
            rowIdentityMapFactory,
            commandBatchPreparerDependencies)
    {
    }

    public override IReadOnlyList<MigrationOperation> GetDifferences(IRelationalModel? source, IRelationalModel? target)
    {
        var operations = base.GetDifferences(source, target)
            .Where(op => !(op is AddForeignKeyOperation))
            .Where(op => !(op is DropForeignKeyOperation))
            .ToList();

        foreach (var operation in operations.OfType<CreateTableOperation>())
            operation.ForeignKeys?.Clear();

        return operations;
    }

 
}