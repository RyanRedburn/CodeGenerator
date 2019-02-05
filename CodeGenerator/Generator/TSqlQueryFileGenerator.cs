using CodeGenerator.Enumeration;
using CodeGenerator.Helper;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Model;
using System;
using System.Linq;
using System.Text;

namespace CodeGenerator.Generator
{
    /// <summary>
    /// A file generator for T-SQL CRUD queries.
    /// </summary>
    public class TSqlQueryFileGenerator : IFileGenerator
    {
        private readonly string _keyNotFound = "--A T-SQL statement was not generated because the application was unable to identify the table key column.";

        public GeneratedFileType OutputFileType { get; } = GeneratedFileType.TSqlQuery;

        public bool QuoteIdentifiers { get; set; } = true;

        /// <summary>
        /// Generates a T-SQL code file (.sql) containing queries for GetByKey, GetAll, Insert, Update, and Delete operations.
        /// </summary>
        /// <param name="tableSpecification">The table specification upon which to base the generated queries.</param>
        /// <returns>A GeneratedFile of type TSql.</returns>
        public GeneratedFile GenerateFile(TableSpecification tableSpecification)
        {
            var keyColumn = FindKeyColumn(tableSpecification);

            var getById = BuildGetByKeyStatement(tableSpecification, keyColumn);

            var getAll = BuildGetAllStatement(tableSpecification);

            var insert = BuildInsertStatement(tableSpecification);

            var update = BuildUpdateStatement(tableSpecification, keyColumn);

            var delete = BuildDeleteStatement(tableSpecification, keyColumn);

            var result = new GeneratedFile
            {
                FileName = tableSpecification.TableName + ".sql",
                FileContents = string.Format(TemplateLibrary.TSql.QueryFile, getById, getAll, insert, update, delete),
                FileType = OutputFileType
            };

            return result;
        }

        /// <summary>
        /// Build a T-SQL delete statement using the given table specification and key column.
        /// </summary>
        /// <param name="tableSpecification">The table to target for the select (get by key) statement.</param>
        /// <param name="keyColumn">The key column to use for the select predicate.</param>
        /// <returns>A string select statement. Empty string if keyColumn is null.</returns>
        private string BuildGetByKeyStatement(TableSpecification tableSpecification, ColumnSpecification keyColumn)
        {
            if (keyColumn == null)
                return _keyNotFound;

            var getColumnList = new StringBuilder();

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                if (getColumnList.Length > 0)
                    getColumnList.Append(", ");

                getColumnList.Append(QuoteName(col.ColumnName));
            }

            return string.Format(TemplateLibrary.TSql.GetByKey, getColumnList.ToString(), QuoteName(tableSpecification.SchemaName), QuoteName(tableSpecification.TableName), QuoteName(keyColumn.ColumnName), keyColumn.ColumnName);
        }

        /// <summary>
        /// Build a T-SQL delete statement using the given table specification.
        /// </summary>
        /// <param name="tableSpecification">The table to target for the select (get all) statement.</param>
        /// <returns>A string select statement.</returns>
        private string BuildGetAllStatement(TableSpecification tableSpecification)
        {
            var getColumnList = new StringBuilder();

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                if (getColumnList.Length > 0)
                    getColumnList.Append(", ");

                getColumnList.Append(QuoteName(col.ColumnName));
            }

            return string.Format(TemplateLibrary.TSql.GetAll, getColumnList.ToString(), QuoteName(tableSpecification.SchemaName), QuoteName(tableSpecification.TableName));
        }

        /// <summary>
        /// Build a T-SQL insert statement using the given table specification.
        /// </summary>
        /// <param name="tableSpecification">The table to target for the insert statement.</param>
        /// <returns>A string insert statement.</returns>
        private string BuildInsertStatement(TableSpecification tableSpecification)
        {
            var insertColumnList = new StringBuilder();
            var insertParameterList = new StringBuilder();

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                if (col.IsIdentity || col.IsComputed)
                    continue;

                // Column list
                if (insertColumnList.Length > 0)
                    insertColumnList.Append(", ");

                insertColumnList.Append(QuoteName(col.ColumnName));

                // Parameter list
                if (insertParameterList.Length > 0)
                    insertParameterList.Append(", ");

                insertParameterList.Append("@" + col.ColumnName);
            }

            return string.Format(TemplateLibrary.TSql.Insert, QuoteName(tableSpecification.SchemaName), QuoteName(tableSpecification.TableName), insertColumnList.ToString(), insertParameterList.ToString());
        }

        /// <summary>
        /// Build a T-SQL update statement using the given table specification and key column.
        /// </summary>
        /// <param name="tableSpecification">The table to target for the update statement.</param>
        /// <param name="keyColumn">The key column to use for the update predicate.</param>
        /// <returns>A string update statement. Empty string if keyColumn is null.</returns>
        private string BuildUpdateStatement(TableSpecification tableSpecification, ColumnSpecification keyColumn)
        {
            if (keyColumn == null)
                return _keyNotFound;

            var updateSetList = new StringBuilder();

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                if (col.IsIdentity || col.IsComputed)
                    continue;

                if (updateSetList.Length > 0)
                    updateSetList.Append(", ");

                updateSetList.Append(QuoteName(col.ColumnName) + " = @" + col.ColumnName);
            }

            return string.Format(TemplateLibrary.TSql.Update, QuoteName(tableSpecification.SchemaName), QuoteName(tableSpecification.TableName), updateSetList.ToString(), QuoteName(keyColumn.ColumnName), keyColumn.ColumnName);
        }

        /// <summary>
        /// Build a T-SQL delete statement using the given table specification and key column.
        /// </summary>
        /// <param name="tableSpecification">The table to target for the delete statement.</param>
        /// <param name="keyColumn">The key column to use for the delete predicate.</param>
        /// <returns>A string delete statement. Empty string if keyColumn is null.</returns>
        private string BuildDeleteStatement(TableSpecification tableSpecification, ColumnSpecification keyColumn)
        {
            if (keyColumn == null)
                return _keyNotFound;

            return string.Format(TemplateLibrary.TSql.Delete, QuoteName(tableSpecification.SchemaName), QuoteName(tableSpecification.TableName), QuoteName(keyColumn.ColumnName), keyColumn.ColumnName);
        }

        /// <summary>
        /// Attempt to find the key column for the given column specification.
        /// </summary>
        /// <param name="tableSpecification">The table specification to analyze.</param>
        /// <returns>The key ColumSpecification, if found.</returns>
        private ColumnSpecification FindKeyColumn(TableSpecification tableSpecification)
        {
            var id = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, "id", StringComparison.OrdinalIgnoreCase));
            if (id == null)
                id = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, tableSpecification.TableName + "id", StringComparison.OrdinalIgnoreCase));
            if (id == null)
                id = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, tableSpecification.TableName + "_id", StringComparison.OrdinalIgnoreCase));

            if (id != null)
                return id;

            var key = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, "key", StringComparison.OrdinalIgnoreCase));
            if (key == null)
                key = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, tableSpecification.TableName + "key", StringComparison.OrdinalIgnoreCase));
            if (key == null)
                key = tableSpecification.ColumnSpecifications.SingleOrDefault(c => string.Equals(c.ColumnName, tableSpecification.TableName + "_key", StringComparison.OrdinalIgnoreCase));

            if (key != null)
                return key;

            var identity = tableSpecification.ColumnSpecifications.SingleOrDefault(c => c.IsIdentity);

            if (identity != null)
                return identity;

            return null;
        }

        /// <summary>
        /// Quotes object names if QuoteIdentifiers is set to true.
        /// </summary>
        /// <param name="objectName">The object name to quote.</param>
        /// <returns>Quoted object name, if applicable.</returns>
        private string QuoteName(string objectName)
        {
            return QuoteIdentifiers ? "[" + objectName + "]" : objectName;
        }
    }
}
