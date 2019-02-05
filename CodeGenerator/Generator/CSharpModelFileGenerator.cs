using CodeGenerator.Enumeration;
using CodeGenerator.Helper;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeGenerator.Generator
{
    /// <summary>
    /// A file generator for C# models.
    /// </summary>
    public class CSharpModelFileGenerator : IFileGenerator
    {
        public GeneratedFileType OutputFileType { get { return GeneratedFileType.CSharpModel; } }

        private List<string> _usingStatements { get; set; }

        public string ModelNameSpace { get; set; }

        public bool AddAnnotations { get; set; } = false;

        public bool OnlyExactMatchForAnnotations { get; set; } = true;

        public CSharpModelFileGenerator()
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
            tableSpecification.TableName = RemoveInvalidCharacters(tableSpecification.TableName);

            var properties = new StringBuilder();

            var addXmlNameSpace = false;
            var addSqlTypeNameSpace = false;

            foreach (var col in tableSpecification.ColumnSpecifications)
            {
                col.ColumnName = RemoveInvalidCharacters(col.ColumnName);

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

            _usingStatements.ForEach(u => { if (usings.Length > 0) usings.Append("\n"); usings.Append(u); });

            if (addXmlNameSpace)
                usings.Append("\nusing System.Linq.Xml;");

            if (addSqlTypeNameSpace)
                usings.Append("\nusing Microsoft.SqlServer.Types;");

            if (AddAnnotations)
                usings.Append("\nusing System.ComponentModel.DataAnnotations;");

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
            string prop = null;
            Type propType = null;
            switch (provider)
            {
                case SpecificationProvider.SqlServer:
                    (prop, propType) = BuildSqlServerPropertyString(columnSpecification);
                    break;
                default:
                    break;
            }

            return AddAnnotations ? BuildPropertyAnnotationString(columnSpecification, propType) + prop : prop;
        }

        /// <summary>
        /// Builds an annotation string for the given column specification.
        /// </summary>
        /// <param name="columnSpecification">The column for which to build an annotation string.</param>
        /// <param name="columnType">The C# type of the column.</param>
        /// <returns>Annotation string.</returns>
        private string BuildPropertyAnnotationString(ColumnSpecification columnSpecification, Type columnType)
        {
            const string whiteSpace = "\n\t\t";

            var basePasswordString = "password";
            var baseEmailString = "email";
            var basePhoneString = "phone";
            var baseCreditCardString = "creditcard";
            var baseZipCodeString = "zip";
            var basePostalCodeString = "postal";
            var emailStrings = new List<string>() { baseEmailString, "emailaddress" };
            var phoneStrings = new List<string>() { basePhoneString, "phonenumber", "homephone", "cellphone" };
            var creditCardStrings = new List<string>() { baseCreditCardString, "creditcardnumber" };
            var postalCodeStrings = new List<string>() { baseZipCodeString, "zipcode", basePostalCodeString, "postalcode" };

            var columnName = new string(Array.FindAll(columnSpecification.ColumnName.ToCharArray(), x => char.IsLetterOrDigit(x)));

            var sb = new StringBuilder();

            // Required
            if (!columnSpecification.IsNullable)
                sb.Append("[Required]" + whiteSpace);

            // Read Only
            if (columnSpecification.IsComputed)
                sb.Append("[ReadOnly]" + whiteSpace);

            // String Length
            if (columnType == typeof(string) && columnSpecification.ColumnLength > 0)
                sb.Append("[StringLength(" + columnSpecification.ColumnLength + ")]" + whiteSpace);

            // Email
            if (columnType == typeof(string)
                && emailStrings.Any(x => x.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                || (!OnlyExactMatchForAnnotations && columnName.Contains(baseEmailString, StringComparison.InvariantCultureIgnoreCase)))
                sb.Append("[DataType(DataType.EmailAddress)]" + whiteSpace);

            // Phone
            if (columnType == typeof(string)
                && phoneStrings.Any(x => x.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                || (!OnlyExactMatchForAnnotations && columnName.Contains("phone", StringComparison.InvariantCultureIgnoreCase)))
                sb.Append("[DataType(DataType.PhoneNumber)]" + whiteSpace);

            // Password
            if (columnType == typeof(string)
                && string.Equals(columnName, basePasswordString, StringComparison.InvariantCultureIgnoreCase)
                || (!OnlyExactMatchForAnnotations && columnName.Contains(basePasswordString, StringComparison.InvariantCultureIgnoreCase)))
                sb.Append("[DataType(DataType.Password)]" + whiteSpace);

            // Credit Card
            if (columnType == typeof(string)
                && creditCardStrings.Any(x => x.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                || (!OnlyExactMatchForAnnotations && columnName.Contains(baseCreditCardString, StringComparison.InvariantCultureIgnoreCase)))
                sb.Append("[DataType(DataType.CreditCard)]" + whiteSpace);

            // Postal Code
            if (columnType == typeof(string)
                && postalCodeStrings.Any(x => x.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                || (!OnlyExactMatchForAnnotations && (columnName.Contains(baseZipCodeString, StringComparison.InvariantCultureIgnoreCase) || columnName.Contains(basePostalCodeString, StringComparison.InvariantCultureIgnoreCase))))
                sb.Append("[DataType(DataType.PostalCode)]" + whiteSpace);

            return sb.ToString();
        }

        /// <summary>
        /// Build a property string based on SQL Server column types. Some non-standard property types may return a null Type.
        /// </summary>
        /// <param name="columnSpecification">The column for which to build a property string.</param>
        /// <returns>Tuple containing the property string and property type.</returns>
        private (string, Type) BuildSqlServerPropertyString(ColumnSpecification columnSpecification)
        {
            var columnType = (SqlServerColumnType)columnSpecification.ColumnType;

            Type type = null; // Note that SQL Server types are left as null. There is no point including an additional library reference when knowing the SQL types is of no value.
            var typeName = string.Empty;
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
                    typeName = "string";
                    type = typeof(string);
                    break;
                case SqlServerColumnType.UniqueIdentifier:
                    typeName = "Guid" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(Guid);
                    break;
                case SqlServerColumnType.Date:
                case SqlServerColumnType.Time:
                case SqlServerColumnType.DateTime:
                case SqlServerColumnType.DateTime2:
                case SqlServerColumnType.DateTimeOffset:
                case SqlServerColumnType.SmallDateTime:
                    typeName = "DateTime" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(DateTime);
                    break;
                case SqlServerColumnType.TinyInt:
                    typeName = "sbyte" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(sbyte);
                    break;
                case SqlServerColumnType.SmallInt:
                    typeName = "short" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(short);
                    break;
                case SqlServerColumnType.Int:
                    typeName = "int" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(int);
                    break;
                case SqlServerColumnType.BigInt:
                case SqlServerColumnType.TimeStamp:
                    typeName = "long" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(long);
                    break;
                case SqlServerColumnType.Real:
                case SqlServerColumnType.Float:
                    typeName = "double" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(double);
                    break;
                case SqlServerColumnType.Sql_Variant:
                    typeName = "object";
                    type = typeof(object);
                    break;
                case SqlServerColumnType.Bit:
                    typeName = "bool" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(bool);
                    break;
                case SqlServerColumnType.Decimal:
                case SqlServerColumnType.Numeric:
                case SqlServerColumnType.Money:
                case SqlServerColumnType.SmallMoney:
                    typeName = "decimal" + (columnSpecification.IsNullable ? "?" : "");
                    type = typeof(decimal);
                    break;
                case SqlServerColumnType.HierarchyId:
                    typeName = "SqlHierarchyId" + (columnSpecification.IsNullable ? "?" : "");
                    break;
                case SqlServerColumnType.Geometry:
                    typeName = "SqlGeometry";
                    break;
                case SqlServerColumnType.Geography:
                    typeName = "SqlGeography";
                    break;
                case SqlServerColumnType.VarBinary:
                case SqlServerColumnType.Binary:
                case SqlServerColumnType.Image:
                    typeName = "byte[]";
                    type = typeof(byte[]);
                    break;
                case SqlServerColumnType.Xml:
                    typeName = "XElement";
                    type = typeof(XElement);
                    break;
                default:
                    typeName = "object";
                    type = typeof(object);
                    break;
            }

            return (string.Format(basePropertyString, typeName, columnSpecification.ColumnName), type);
        }

        /// <summary>
        /// Remove invalid characters from a string that is intended to be used as a C# member name (e.g., class property, etc.).
        /// </summary>
        /// <param name="target">String to modify.</param>
        /// <returns>Target string with invalid characters removed.</returns>
        private string RemoveInvalidCharacters(string target)
        {
            if (string.IsNullOrWhiteSpace(target))
                return target;

            return new string(Array.FindAll(target.ToCharArray(), x => char.IsLetterOrDigit(x) || x == '_'));
        }
    }
}
