namespace DuckHouse.Ui.Shared.Catalogs;

public record CatalogInfoDto(IReadOnlyList<CatalogMetadataEntryDto> Metadata);

public record CatalogMetadataEntryDto(
    string Key,
    string Value,
    string? Scope,
    long? ScopeId);

public record CatalogMetadataDto(
    string Name,
    IReadOnlyList<SchemaDto> Schemas);

public record SchemaDto(
    string Name,
    IReadOnlyList<TableDto> Tables);

public record TableDto(
    string Name,
    string Type,
    IReadOnlyList<ColumnDto> Columns,
    string? Comment);

public record ColumnDto(
    string Name,
    string DataType,
    bool IsNullable,
    int OrdinalPosition,
    string? Comment,
    int? PartitionKeyIndex,
    string? PartitionTransform);
