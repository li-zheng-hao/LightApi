namespace LightApi.EFCore.EFCore.Internal;

internal class KeyEntryModel
{
    public string PropertyName { get; set; } = string.Empty;

    public string ColumnName { get; set; } = string.Empty;

    public object Value { get; set; } = default!;
}