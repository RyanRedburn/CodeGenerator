using CodeGenerator.Enumeration;
using CodeGenerator.Interface.Generator;
using CodeGenerator.Model;
using System.Collections.Generic;

namespace CodeGenerator.Interface.Service
{
    /// <summary>
    /// Service class used to drive code file generation.
    /// </summary>
    public interface IFileGenerationService
    {
        /// <summary>
        /// The collection of file generators used by the service.
        /// </summary>
        IEnumerable<IFileGenerator> FileGenerators { get; }

        /// <summary>
        /// Adds a file generator to the service. Duplicate (based on output file type) file generators cannot be added.
        /// </summary>
        /// <param name="fileGenerator">IFileGenerator to add.</param>
        void AddFileGenerator(IFileGenerator fileGenerator);

        /// <summary>
        /// Removes a file generator from the service.
        /// </summary>
        /// <param name="fileType">The file type of generator to remove.</param>
        void RemoveFileGenerator(GeneratedFileType fileType);

        /// <summary>
        /// Generates code files using the file generators contained by the service.
        /// </summary>
        /// <returns>Dictionary of output results.</returns>
        Dictionary<GeneratedFileType, IEnumerable<GeneratedFile>> GenerateFiles();
    }
}
