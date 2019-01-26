using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeGenerator.Test.Generator
{
    [TestClass]
    public class TSqlFileGeneratorTest
    {
        [TestMethod]
        public void OutputFileType()
        {
            var generator = new TSqlFileGenerator() as IFileGenerator;

            Assert.AreEqual(GeneratedFileType.TSql, generator.OutputFileType);
        }

        [TestMethod]
        public void GenerateFileUnquoted()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFile(GeneratedFileType.TSql, quoteIdentifiers: false);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new TSqlFileGenerator { QuoteIdentifiers = false } as IFileGenerator;

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileQuoted()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFile(GeneratedFileType.TSql, quoteIdentifiers: true);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new TSqlFileGenerator { QuoteIdentifiers = true } as IFileGenerator;

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }
    }
}
