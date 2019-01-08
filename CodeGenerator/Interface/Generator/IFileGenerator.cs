using CodeGenerator.Enumeration;
using CodeGenerator.Model;

namespace CodeGenerator.Interface.Generator
{
    /// <summary>
    /// Class used to generate a code file for a given output type and table specification.
    /// </summary>
    public interface IFileGenerator
    {
        /// <summary>
        /// The type of file output by the generator.
        /// </summary>
        GeneratedFileType OutputFileType { get; }

        /// <summary>
        /// Generates an output file for the given TableSpecification.
        /// </summary>
        /// <param name="tableSpecification">The TableSpecification to use for file generation.</param>
        /// <returns>GeneratedFile</returns>
        GeneratedFile GenerateFile(TableSpecification tableSpecification);
    }
}
