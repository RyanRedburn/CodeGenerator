using CodeGenerator.Enumeration;
using CodeGenerator.Model;
using System.Collections.Generic;

namespace CodeGenerator.Test.Utility
{
    internal static class TestUtility
    {
        internal static TableSpecification GetCustomerTableSpec()
        {
            return new TableSpecification
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
        }

        internal static GeneratedFile GetExpectedCustomerFile(GeneratedFileType fileType, string nameSpace = null, bool quoteIdentifiers = true)
        {
            switch (fileType)
            {
                case GeneratedFileType.CSharp:
                    return new GeneratedFile
                    {
                        FileName = "Customer.cs",
                        FileType = GeneratedFileType.CSharp,
                        FileContents = "using System;\nusing System.Collections.Generic;\nusing System.Text;\n\nnamespace " + (nameSpace ?? "") + "\n{\n\tpublic class Customer\n\t{\n\t\tpublic int Id { get; set; }\n\n\t\tpublic string FirstName { get; set; }\n\n\t\tpublic string LastName { get; set; }\n\t}\n}"
                    };
                case GeneratedFileType.TSql:
                    return new GeneratedFile
                    {
                        FileName = "Customer.sql",
                        FileType = GeneratedFileType.TSql,
                        FileContents = quoteIdentifiers
                            ? "--GetByKey\nSELECT [Id], [FirstName], [LastName] FROM [dbo].[Customer] WHERE [Id] = @Id;\n\n--GetAll\nSELECT [Id], [FirstName], [LastName] FROM [dbo].[Customer];\n\n--Insert\nINSERT [dbo].[Customer]([FirstName], [LastName]) VALUES (@FirstName, @LastName);\n\n--Update\nUPDATE [dbo].[Customer] SET [FirstName] = @FirstName, [LastName] = @LastName WHERE [Id] = @Id;\n\n--Delete\nDELETE [dbo].[Customer] WHERE [Id] = @Id;"
                            : "--GetByKey\nSELECT Id, FirstName, LastName FROM dbo.Customer WHERE Id = @Id;\n\n--GetAll\nSELECT Id, FirstName, LastName FROM dbo.Customer;\n\n--Insert\nINSERT dbo.Customer(FirstName, LastName) VALUES (@FirstName, @LastName);\n\n--Update\nUPDATE dbo.Customer SET FirstName = @FirstName, LastName = @LastName WHERE Id = @Id;\n\n--Delete\nDELETE dbo.Customer WHERE Id = @Id;"
                    };
                default:
                    return null;
            }
        }
    }
}
