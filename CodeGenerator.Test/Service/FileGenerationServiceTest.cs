using CodeGenerator.Enumeration;
using CodeGenerator.Generator;
using CodeGenerator.Interface.Repository;
using CodeGenerator.Model;
using CodeGenerator.Service;
using CodeGenerator.Test.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Test.Service
{
    [TestClass]
    public class FileGenerationServiceTest
    {
        [TestMethod]
        public void AddFileGenerator()
        {
            var repository = new Mock<ISpecificationRepository>();
            var logger = new Mock<ILogger>();
            var service = new FileGenerationService(logger.Object, repository.Object);

            service.AddFileGenerator(new CSharpModelFileGenerator());

            var generator = service.FileGenerators.SingleOrDefault(x => x.OutputFileType == GeneratedFileType.CSharpModel);

            Assert.IsNotNull(generator);
        }

        [TestMethod]
        public void AddDuplicateFileGenerator()
        {
            var repository = new Mock<ISpecificationRepository>();
            var logger = new Mock<ILogger>();
            var service = new FileGenerationService(logger.Object, repository.Object);

            service.AddFileGenerator(new CSharpModelFileGenerator());

            var ex = Assert.ThrowsException<InvalidOperationException>(() => service.AddFileGenerator(new CSharpModelFileGenerator()));
            Assert.AreEqual(string.Format("IFileGenerator with output type {0} is already present in the service collection.", GeneratedFileType.CSharpModel), ex.Message);
        }

        [TestMethod]
        public void RemoveFileGenerator()
        {
            var repository = new Mock<ISpecificationRepository>();
            var logger = new Mock<ILogger>();
            var service = new FileGenerationService(logger.Object, repository.Object);

            service.AddFileGenerator(new CSharpModelFileGenerator());

            service.RemoveFileGenerator(GeneratedFileType.CSharpModel);

            var generator = service.FileGenerators.SingleOrDefault(x => x.OutputFileType == GeneratedFileType.CSharpModel);

            Assert.IsNull(generator);
            Assert.AreEqual(0, service.FileGenerators.Count());
        }

        [TestMethod]
        public void GenerateFiles()
        {
            var repository = new Mock<ISpecificationRepository>();
            repository.Setup(x => x.GetAll()).Returns(new List<TableSpecification> { TestUtility.GetCustomerTableSpec() });
            var logger = new Mock<ILogger>();
            var service = new FileGenerationService(logger.Object, repository.Object);
            service.AddFileGenerator(new CSharpModelFileGenerator());
            service.AddFileGenerator(new TSqlQueryFileGenerator());

            var result = service.GenerateFiles();

            Assert.IsTrue(result.ContainsKey(GeneratedFileType.CSharpModel));
            Assert.IsTrue(result.ContainsKey(GeneratedFileType.TSqlQuery));

            IEnumerable<GeneratedFile> csharp = null;
            IEnumerable<GeneratedFile> tsql = null;

            result.TryGetValue(GeneratedFileType.CSharpModel, out csharp);
            result.TryGetValue(GeneratedFileType.TSqlQuery, out tsql);

            Assert.AreEqual(1, csharp.Count());
            Assert.AreEqual(1, tsql.Count());
        }
    }
}
