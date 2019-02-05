namespace CodeGenerator.Helper
{
    /// <summary>
    /// The QueryLibrary contains the definitions of application queries.
    /// </summary>
    internal static class QueryLibrary
    {
        internal static class MSSQL
        {
            internal static string GetTableSpecification =
                @"SELECT s.[name] AS SchemaName, t.[name] AS TableName, t.[object_id] AS ObjectId
                FROM sys.tables AS t
                JOIN sys.schemas AS s ON s.[schema_id] = t.[schema_id]
                WHERE t.[type] = 'U' AND t.is_ms_shipped = 0 AND t.is_external = 0;";

            internal static string GetColumnSpecification =
                @"SELECT c.[object_id] AS ObjectId, c.[name] AS ColumnName, c.user_type_id AS ColumnType,
                c.is_computed AS IsComputed, c.is_nullable AS IsNullable, c.is_identity AS IsIdentity,
                c.max_length AS ColumnLength
                FROM sys.tables AS t
                JOIN sys.columns AS c ON c.[object_id] = t.[object_id]
                WHERE t.[type] = 'U' AND t.is_ms_shipped = 0 AND t.is_external = 0;";
        }
    }
}
