using CodeGenerator.Enumeration;
using CodeGenerator.Model;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="addExtendedProps">Whether or not to add extended properties (e.g., Password, EmailAddress, etc.).</param>
        /// <returns>Customer TableSpecification.</returns>
        internal static TableSpecification GetCustomerTableSpec(bool addXmlProp = false, bool addSqlSpecificProps = false, bool addExtendedProps = false)
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
                        IsNullable = false,
                        ColumnLength = 50
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "LastName",
                        ColumnType = 167,
                        IsNullable = false,
                        ColumnLength = 50
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "ContactPhone",
                        ColumnType = 167,
                        IsNullable = true,
                        ColumnLength = 12
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "ZipCode",
                        ColumnType = 167,
                        IsNullable = true,
                        ColumnLength = 12
                    }
                }
            };

            if (addExtendedProps)
                spec.ColumnSpecifications.AddRange(new List<ColumnSpecification>()
                {
                    new ColumnSpecification
                    {
                        ColumnName = "Password",
                        ColumnType = 167,
                        IsNullable = true,
                        ColumnLength = 50
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "EmailAddress",
                        ColumnType = 167,
                        IsNullable = true,
                        ColumnLength = 100
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "CreditCardNumber",
                        ColumnType = 167,
                        IsNullable = true,
                        ColumnLength = 16
                    },
                    new ColumnSpecification
                    {
                        ColumnName = "Age",
                        ColumnType = 56,
                        IsComputed = true
                    }
                });

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
                    ? "--GetByKey\nSELECT [Id], [FirstName], [LastName], [ContactPhone], [ZipCode] FROM [dbo].[Customer] WHERE [Id] = @Id;\n\n--GetAll\nSELECT [Id], [FirstName], [LastName], [ContactPhone], [ZipCode] FROM [dbo].[Customer];\n\n--Insert\nINSERT [dbo].[Customer]([FirstName], [LastName], [ContactPhone], [ZipCode]) VALUES (@FirstName, @LastName, @ContactPhone, @ZipCode);\n\n--Update\nUPDATE [dbo].[Customer] SET [FirstName] = @FirstName, [LastName] = @LastName, [ContactPhone] = @ContactPhone, [ZipCode] = @ZipCode WHERE [Id] = @Id;\n\n--Delete\nDELETE [dbo].[Customer] WHERE [Id] = @Id;"
                    : "--GetByKey\nSELECT Id, FirstName, LastName, ContactPhone, ZipCode FROM dbo.Customer WHERE Id = @Id;\n\n--GetAll\nSELECT Id, FirstName, LastName, ContactPhone, ZipCode FROM dbo.Customer;\n\n--Insert\nINSERT dbo.Customer(FirstName, LastName, ContactPhone, ZipCode) VALUES (@FirstName, @LastName, @ContactPhone, @ZipCode);\n\n--Update\nUPDATE dbo.Customer SET FirstName = @FirstName, LastName = @LastName, ContactPhone = @ContactPhone, ZipCode = @ZipCode WHERE Id = @Id;\n\n--Delete\nDELETE dbo.Customer WHERE Id = @Id;"
            };
        }

        /// <summary>
        /// Returns the expected customer C# file based on the GetCustomerTableSpec() method.
        /// </summary>
        /// <param name="nameSpace">The name space to use for the file.</param>
        /// <param name="addXmlProp">Whether or not an XML property should be included.</param>
        /// <param name="addSqlSpecificProps">Whether or not SQL Server specific property types should be included (Geography, Geometry, etc.).</param>
        /// <param name="addAnnotations">Whether or not to add data annotations to properties.</param>
        /// <param name="onlyExactMatchForAnnotations">Whether or not to add annotations for data types based on approximation.</param>
        /// <param name="addExtendedProps">Whether or not to add extended properties (e.g., Password, EmailAddress, etc.).</param>
        /// <returns>Customer C# file.</returns>
        internal static GeneratedFile GetExpectedCustomerFileCSharp(string nameSpace = null, bool addXmlProp = false, bool addSqlSpecificProps = false, bool addAnnotations = false, bool onlyExactMatchForAnnotations = true, bool addExtendedProps = false)
        {
            var contents = new StringBuilder();

            // Usings
            contents.Append("using System;\nusing System.Collections.Generic;\nusing System.Text;\n");
            if (addXmlProp)
                contents.Append("using System.Linq.Xml;\n");
            if (addSqlSpecificProps)
                contents.Append("using Microsoft.SqlServer.Types;\n");
            if (addAnnotations)
                contents.Append("using System.ComponentModel.DataAnnotations;\n");

            // Class
            contents.Append("\nnamespace " + (nameSpace ?? ""));
            contents.Append("\n{\n\tpublic class Customer\n\t{\n");

            // Properties
            if (addAnnotations)
                contents.Append("\t\t[Required]\n");
            contents.Append("\t\tpublic int Id { get; set; }\n\n");
            if (addAnnotations)
                contents.Append("\t\t[Required]\n\t\t[StringLength(50)]\n");
            contents.Append("\t\tpublic string FirstName { get; set; }\n\n");
            if (addAnnotations)
                contents.Append("\t\t[Required]\n\t\t[StringLength(50)]\n");
            contents.Append("\t\tpublic string LastName { get; set; }\n\n");
            if (addAnnotations)
                contents.Append("\t\t[StringLength(12)]\n");
            if (addAnnotations && !onlyExactMatchForAnnotations)
                contents.Append("\t\t[DataType(DataType.PhoneNumber)]\n");
            contents.Append("\t\tpublic string ContactPhone { get; set; }\n\n");
            if (addAnnotations)
                contents.Append("\t\t[StringLength(12)]\n\t\t[DataType(DataType.PostalCode)]\n");
            contents.Append("\t\tpublic string ZipCode { get; set; }");
            if (addExtendedProps)
                contents.Append("\n\n\t\t[StringLength(50)]\n\t\t[DataType(DataType.Password)]\n\t\tpublic string Password { get; set; }\n\n\t\t[StringLength(100)]\n\t\t[DataType(DataType.EmailAddress)]\n\t\tpublic string EmailAddress { get; set; }\n\n\t\t[StringLength(16)]\n\t\t[DataType(DataType.CreditCard)]\n\t\tpublic string CreditCardNumber { get; set; }\n\n\t\t[ReadOnly]\n\t\tpublic int? Age { get; set; }");
            if (addXmlProp)
                contents.Append("\n\n\t\tpublic XElement Preferences { get; set; }");
            if (addSqlSpecificProps)
                contents.Append("\n\n\t\tpublic SqlHierarchyId? HierarchyId { get; set; }\n\n\t\tpublic SqlGeography Location { get; set; }\n\n\t\tpublic SqlGeometry DropboxDimensions { get; set; }");

            contents.Append("\n\t}\n}");

            return new GeneratedFile
            {
                FileName = "Customer.cs",
                FileType = GeneratedFileType.CSharp,
                FileContents = contents.ToString()
            };
        }
    }
}
