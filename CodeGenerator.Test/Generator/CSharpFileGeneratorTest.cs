using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Interface.Generator;
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
            var generator = new CSharpFileGenerator() as IFileGenerator;

            Assert.AreEqual(GeneratedFileType.CSharp, generator.OutputFileType);
        }

        [TestMethod]
        public void GenerateFile()
        {
            var expectedFile = TestUtility.GetExpectedCustomerFile(GeneratedFileType.CSharp, nameSpace: "Test");

            var tableSpec = TestUtility.GetCustomerTableSpec();

            var generator = new CSharpFileGenerator { ModelNameSpace = "Test" } as IFileGenerator;

            var file = generator.GenerateFile(tableSpec);

            Assert.AreEqual(expectedFile.FileName, file.FileName);
            Assert.AreEqual(expectedFile.FileType, file.FileType);
            Assert.AreEqual(expectedFile.FileContents, file.FileContents);
        }
    }
}
