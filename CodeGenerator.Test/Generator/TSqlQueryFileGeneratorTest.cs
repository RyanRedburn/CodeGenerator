using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeGenerator.Test.Generator
{
    [TestClass]
    public class TSqlQueryFileGeneratorTest
    {
        [TestMethod]
        public void OutputFileType()
        {
            var generator = new TSqlQueryFileGenerator();

            Assert.AreEqual(GeneratedFileType.TSqlQuery, generator.OutputFileType);
        }

        [TestMethod]
        public void GenerateFileUnquoted()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileTSqlQuery(false);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new TSqlQueryFileGenerator { QuoteIdentifiers = false };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileQuoted()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileTSqlQuery(true);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new TSqlQueryFileGenerator { QuoteIdentifiers = true };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }
    }
}
