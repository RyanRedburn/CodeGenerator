using CodeGenerator.Enumeration;

namespace CodeGenerator.Model
{
    /// <summary>
    /// A class used to represent code generation output.
    /// </summary>
    public class GeneratedFile
    {
        public string FileName { get; set; }

        public string FileContents { get; set; }

        public GeneratedFileType FileType { get; set; }
    }
}
