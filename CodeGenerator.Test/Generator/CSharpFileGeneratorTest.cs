using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeGenerator.Test.Generator
{
    [TestClass]
    public class CSharpFileGeneratorTest
    {
        [TestMethod]
        public void OutputFileType()
        {
            var generator = new CSharpFileGenerator();

            Assert.AreEqual(GeneratedFileType.CSharp, generator.OutputFileType);
        }

        [TestMethod]
        public void GenerateFile()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharp(nameSpace: "Test");

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new CSharpFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithXmlProperty()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharp(nameSpace: "Test", addXmlProp: true);

            var tableSpec = TestUtility.GetCustomerTableSpec(addXmlProp: true);

            var generator = new CSharpFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithSqlSpecificProps()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharp(nameSpace: "Test", addSqlSpecificProps: true);

            var tableSpec = TestUtility.GetCustomerTableSpec(addSqlSpecificProps: true);

            var generator = new CSharpFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }
    }
}
