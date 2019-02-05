using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Test.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeGenerator.Test.Generator
{
    [TestClass]
    public class CSharpModelFileGeneratorTest
    {
        [TestMethod]
        public void OutputFileType()
        {
            var generator = new CSharpModelFileGenerator();

            Assert.AreEqual(GeneratedFileType.CSharpModel, generator.OutputFileType);
        }

        [TestMethod]
        public void GenerateFile()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel(nameSpace: "Test");

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithXmlProperty()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel(nameSpace: "Test", addXmlProp: true);

            var tableSpec = TestUtility.GetCustomerTableSpec(addXmlProp: true);

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithSqlSpecificProps()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel(nameSpace: "Test", addSqlSpecificProps: true);

            var tableSpec = TestUtility.GetCustomerTableSpec(addSqlSpecificProps: true);

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test" };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithAnnotations()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel(nameSpace: "Test", addAnnotations: true);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test", AddAnnotations = true, OnlyExactMatchForAnnotations = true };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithApproxAnnotations()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel(nameSpace: "Test", addAnnotations: true, onlyExactMatchForAnnotations: false);

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test", AddAnnotations = true, OnlyExactMatchForAnnotations = false };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }

        [TestMethod]
        public void GenerateFileWithExtendedProperties()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFileCSharpModel("Test", true, true, true, false, true);

            var tableSpec = TestUtility.GetCustomerTableSpec(true, true, true);

            var generator = new CSharpModelFileGenerator { ModelNameSpace = "Test", AddAnnotations = true, OnlyExactMatchForAnnotations = false };

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }
    }
}
