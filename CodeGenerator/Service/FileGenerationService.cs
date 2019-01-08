using CodeGenerator.Enumeration;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Interface.Repository;
using CodeGenerator.Interface.Service;
using CodeGenerator.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Service
{
    /// <summary>
    /// Service class used to drive code file generation.
    /// </summary>
    public class FileGenerationService : IFileGenerationService
    {
        private readonly ILogger _logger;

        private readonly ISpecificationRepository _specificationRepository;

        private readonly List<IFileGenerator> _fileGenerators;

        /// <summary>
        /// The collection of file generators used by the service.
        /// </summary>
        public IEnumerable<IFileGenerator> FileGenerators { get { return _fileGenerators; } }

        public FileGenerationService(ILogger logger, ISpecificationRepository specificationRepository)
        {
            _logger = logger;
            _specificationRepository = specificationRepository;
            _fileGenerators = new List<IFileGenerator>();
        }

        /// <summary>
        /// Adds a file generator to the service. Duplicate (based on output file type) file generators cannot be added.
        /// </summary>
        /// <param name="fileGenerator">IFileGenerator to add.</param>
        public void AddFileGenerator(IFileGenerator fileGenerator)
        {
            if (_fileGenerators.Any(fg => fg.OutputFileType == fileGenerator.OutputFileType))
                throw new InvalidOperationException(string.Format("IFileGenerator with output type {0} is already present in the service collection.", fileGenerator.OutputFileType));

            _fileGenerators.Add(fileGenerator);
        }

        /// <summary>
        /// Removes a file generator from the service.
        /// </summary>
        /// <param name="fileType">The file type of generator to remove.</param>
        public void RemoveFileGenerator(GeneratedFileType fileType)
        {
            var fileGenerator = _fileGenerators.SingleOrDefault(fg => fg.OutputFileType == fileType);
            if (fileGenerator != null)
                _fileGenerators.Remove(fileGenerator);
        }

        /// <summary>
        /// Generates code files using the file generators contained by the service.
        /// </summary>
        /// <returns>Dictionary of output results.</returns>
        public Dictionary<GeneratedFileType, IEnumerable<GeneratedFile>> GenerateFiles()
        {
            try
            {
                var specifications = _specificationRepository.GetAll();

                var result = new Dictionary<GeneratedFileType, IEnumerable<GeneratedFile>>();

                foreach (var generator in _fileGenerators)
                {
                    var files = new List<GeneratedFile>();

                    foreach (var spec in specifications)
                    {
                        try
                        {
                            files.Add(generator.GenerateFile(spec));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Exception during file generation for object {table}.", new { table = (spec.SchemaName + "." + spec.TableName) });
                        }
                    }

                    result.Add(generator.OutputFileType, files);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occured in ModelTemplateService.GenerateTemplates().");
                throw;
            }
        }
    }
}
