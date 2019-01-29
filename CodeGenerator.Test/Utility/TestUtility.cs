using CodeGenerator.Enumeration;
using CodeGenerator.Model;
using System.Collections.Generic;

namespace CodeGenerator.Test.Utility
{
    /// <summary>
    /// Utility class used for encapsulating common unit test operations.
    /// </summary>
    internal static class TestUtility
    {
        /// <summary>
        /// Returns a simple customer table specification.
        /// </summary>
        /// <param name="addXmlProp">Whether or not an XML property should be included.</param>
        /// <param name="addSqlSpecificProps">Whether or not SQL Server specific property types should be included (Geography, Geometry, etc.).</param>
        /// <returns>Customer TableSpecification.</returns>
        internal static TableSpecification GetCustomerTableSpec(bool addXmlProp = false, bool addSqlSpecificProps = false)
        {
            var spec = new TableSpecification
            {
                TableName = "Customer",
                SchemaName = "dbo",
                TableOrigin = SpecificationProvider.SqlServer,
                ColumnSpecifications = new List<ColumnSpecification>()
                {
                    new ColumnSpecification
                    {
                        ColumnName = "Id",
                        ColumnType = 56,
                        IsNullable = false,
                        IsIdentity = true
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "FirstName",
                        ColumnType = 167,
                        IsNullable = false
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "LastName",
                        ColumnType = 167,
                        IsNullable = false
                    }
                }
            };

            if (addXmlProp)
                spec.ColumnSpecifications.Add(new ColumnSpecification
                {
                    ColumnName = "Preferences",
                    ColumnType = 241,
                    IsNullable = true
                });

            if (addSqlSpecificProps)
                spec.ColumnSpecifications.AddRange(new List<ColumnSpecification>()
                {
                    new ColumnSpecification
                    {
                        ColumnName = "HierarchyId",
                        ColumnType = 128,
                        IsNullable = true,
                        IsIdentity = false
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "Location",
                        ColumnType = 130,
                        IsNullable = true
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "DropboxDimensions",
                        ColumnType = 129,
                        IsNullable = true
                    }
                });

            return spec;
        }

        /// <summary>
        /// Returns the expected customer T-SQL file based on the GetCustomerTableSpec() method.
        /// </summary>
        /// <param name="quoteIdentifiers">Whether or not to quote identifiers.</param>
        /// <returns>Customer T-SQL file.</returns>
        internal static GeneratedFile GetExpectedCustomerFileTSql(bool quoteIdentifiers = true)
        {
            return new GeneratedFile
            {
                FileName = "Customer.sql",
                FileType = GeneratedFileType.TSql,
                FileContents = quoteIdentifiers
                    ? "--GetByKey\nSELECT [Id], [FirstName], [LastName] FROM [dbo].[Customer] WHERE [Id] = @Id;\n\n--GetAll\nSELECT [Id], [FirstName], [LastName] FROM [dbo].[Customer];\n\n--Insert\nINSERT [dbo].[Customer]([FirstName], [LastName]) VALUES (@FirstName, @LastName);\n\n--Update\nUPDATE [dbo].[Customer] SET [FirstName] = @FirstName, [LastName] = @LastName WHERE [Id] = @Id;\n\n--Delete\nDELETE [dbo].[Customer] WHERE [Id] = @Id;"
                    : "--GetByKey\nSELECT Id, FirstName, LastName FROM dbo.Customer WHERE Id = @Id;\n\n--GetAll\nSELECT Id, FirstName, LastName FROM dbo.Customer;\n\n--Insert\nINSERT dbo.Customer(FirstName, LastName) VALUES (@FirstName, @LastName);\n\n--Update\nUPDATE dbo.Customer SET FirstName = @FirstName, LastName = @LastName WHERE Id = @Id;\n\n--Delete\nDELETE dbo.Customer WHERE Id = @Id;"
            };
        }

        /// <summary>
        /// Returns the expected customer C# file based on the GetCustomerTableSpec() method.
        /// </summary>
        /// <param name="nameSpace">The name space to use for the file.</param>
        /// <param name="addXmlProp">Whether or not an XML property should be included.</param>
        /// <param name="addSqlSpecificProps">Whether or not SQL Server specific property types should be included (Geography, Geometry, etc.).</param>
        /// <returns>Customer C# file.</returns>
        internal static GeneratedFile GetExpectedCustomerFileCSharp(string nameSpace = null, bool addXmlProp = false, bool addSqlSpecificProps = false)
        {
            return new GeneratedFile
            {
                FileName = "Customer.cs",
                FileType = GeneratedFileType.CSharp,
                FileContents = "using System;\nusing System.Collections.Generic;\nusing System.Text;\n"
                    + (addXmlProp ? "using System.Linq.Xml;\n" : "")
                    + (addSqlSpecificProps ? "using Microsoft.SqlServer.Types;\n" : "")
                    + "\nnamespace " + (nameSpace ?? "")
                    + "\n{\n\tpublic class Customer\n\t{\n\t\tpublic int Id { get; set; }\n\n\t\tpublic string FirstName { get; set; }\n\n\t\tpublic string LastName { get; set; }"
                    + (addXmlProp ? "\n\n\t\tpublic XElement Preferences { get; set; }" : "")
                    + (addSqlSpecificProps ? "\n\n\t\tpublic SqlHierarchyId? HierarchyId { get; set; }\n\n\t\tpublic SqlGeography Location { get; set; }\n\n\t\tpublic SqlGeometry DropboxDimensions { get; set; }" : "")
                    + "\n\t}\n}"
            };
        }
    }
}
