using CodeGenerator.Enumeration;
using CodeGenerator.Helper;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Model;
using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Generator
{
    /// <summary>
    /// A file generator for C# (Microsoft .Net).
    /// </summary>
    public class CSharpFileGenerator : IFileGenerator
    {
        public GeneratedFileType OutputFileType { get { return GeneratedFileType.CSharp; } }

        private List<string> _usingStatements { get; set; }

        public string ModelNameSpace { get; set; }

        public CSharpFileGenerator()
        {
            _usingStatements = new List<string>
            {
                "using System;",
                "using System.Collections.Generic;",
                "using System.Text;"
            };
        }

        /// <summary>
        /// Generates a C# code file (.cs) containing a class definition matching the given table specification.
        /// </summary>
        /// <param name="tableSpecification">The table specification upon which to base the C# class file.</param>
        /// <returns>A GeneratedFile of type CSharp.</returns>
        public GeneratedFile GenerateFile(TableSpecification tableSpecification)
        {
            var properties = new StringBuilder();

            var addXmlNameSpace = false;
            var addSqlTypeNameSpace = false;

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                if (properties.Length > 0)
                    properties.Append("\n\n\t\t");

                properties.Append(BuildPropertyString(col, tableSpecification.TableOrigin));

                // There are using statements that potentially need to be added for certain SQL Server column types.
                if (tableSpecification.TableOrigin == SpecificationProvider.SqlServer)
                {
                    if (!addXmlNameSpace && (SqlServerColumnType)col.ColumnType == SqlServerColumnType.Xml)
                        addXmlNameSpace = true;

                    if (!addSqlTypeNameSpace &&
                        ((SqlServerColumnType)col.ColumnType == SqlServerColumnType.HierarchyId
                        || (SqlServerColumnType)col.ColumnType == SqlServerColumnType.Geometry
                        || (SqlServerColumnType)col.ColumnType == SqlServerColumnType.Geography))
                        addSqlTypeNameSpace = true;
                }
            }

            var usings = new StringBuilder();

            _usingStatements.ForEach(u => { if (usings.Length > 0) usings.AppendLine(); usings.Append(u); });

            if (addXmlNameSpace)
                usings.AppendLine("using System.Linq.Xml;");

            if (addXmlNameSpace)
                usings.AppendLine("using System.Linq.Xml;");

            var result = new GeneratedFile
            {
                FileName = tableSpecification.TableName + ".cs",
                FileContents = string.Format(TemplateLibrary.CSharp.ClassFile, usings.ToString(), ModelNameSpace, tableSpecification.TableName, properties.ToString()),
                FileType = OutputFileType
            };

            return result;
        }

        /// <summary>
        /// Build a property string for the given column specification.
        /// </summary>
        /// <param name="columnSpecification">The column for which to build a property string.</param>
        /// <param name="provider">The column specification provider (e.g., SQL Server).</param>
        /// <returns>Property string.</returns>
        private string BuildPropertyString(ColumnSpecification columnSpecification, SpecificationProvider provider)
        {
            switch (provider)
            {
                case SpecificationProvider.SqlServer:
                    return BuildSqlServerPropertyString(columnSpecification);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Build a property string based on SQL Server column types.
        /// </summary>
        /// <param name="columnSpecification">The column for which to build a property string.</param>
        /// <returns>Property string.</returns>
        private string BuildSqlServerPropertyString(ColumnSpecification columnSpecification)
        {
            var columnType = (SqlServerColumnType)columnSpecification.ColumnType;

            var type = string.Empty;
            var basePropertyString = "public {0} {1} {{ get; set; }}";

            switch (columnType)
            {
                case SqlServerColumnType.Char:
                case SqlServerColumnType.VarChar:
                case SqlServerColumnType.Text:
                case SqlServerColumnType.NText:
                case SqlServerColumnType.NVarChar:
                case SqlServerColumnType.NChar:
                case SqlServerColumnType.SysName:
                    type = "string";
                    break;
                case SqlServerColumnType.UniqueIdentifier:
                    type = "Guid" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Date:
                case SqlServerColumnType.Time:
                case SqlServerColumnType.DateTime:
                case SqlServerColumnType.DateTime2:
                case SqlServerColumnType.DateTimeOffset:
                case SqlServerColumnType.SmallDateTime:
                    type = "DateTime" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.TinyInt:
                    type = "sbyte" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.SmallInt:
                    type = "short" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Int:
                    type = "int" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.BigInt:
                case SqlServerColumnType.TimeStamp:
                    type = "long" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Real:
                case SqlServerColumnType.Float:
                    type = "double" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Sql_Variant:
                    type = "object";
                    break;
                case SqlServerColumnType.Bit:
                    type = "bool" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Decimal:
                case SqlServerColumnType.Numeric:
                case SqlServerColumnType.Money:
                case SqlServerColumnType.SmallMoney:
                    type = "decimal" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.HierarchyId:
                    type = "SqlHierarchyId" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Geometry:
                    type = "SqlGeometry";
                    break;
                case SqlServerColumnType.Geography:
                    type = "SqlGeography";
                    break;
                case SqlServerColumnType.VarBinary:
                case SqlServerColumnType.Binary:
                case SqlServerColumnType.Image:
                    type = "byte[]";
                    break;
                case SqlServerColumnType.Xml:
                    type = "XElement";
                    break;
                default:
                    type = "object";
                    break;
            }

            return string.Format(basePropertyString, type, columnSpecification.ColumnName);
        }
    }
}
