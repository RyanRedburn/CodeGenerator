namespace CodeGenerator.Command
{
    public enum ConnectionType
    {
        SqlServer = 1
    }

    public class ApplicationConfiguration
    {
        #region Fields

        private string _codeDirectory;

        #endregion

        #region Properties

        public string CodeDirectory
        {
            get { return _codeDirectory; }
            set
            {
                _codeDirectory = value;

                if (!string.IsNullOrWhiteSpace(_codeDirectory) && !_codeDirectory.EndsWith("\\"))
                    _codeDirectory += "\\";
            }
        }

        public bool RequestCodeDirectoryOnExec
        {
            get
            {
                return string.IsNullOrWhiteSpace(_codeDirectory);
            }
        }

        public string SourceConnectionString { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public bool RequestFullConnectionOnExec { get; set; } = true;

        public bool RequestCatalogOnlyOnExec { get; set; } = false;

        public CSharpModelConfiguration CSharpModelConfiguration { get; set; } = new CSharpModelConfiguration();

        public TSqlQueryConfiguration TSqlQueryConfiguration { get; set; } = new TSqlQueryConfiguration();

        #endregion
    }

    public class CSharpModelConfiguration
    {
        public bool Active { get; set; } = false;

        public string ModelNameSpace { get; set; }

        public bool RequestNameSpaceOnExec { get; set; } = false;

        public bool AddAnnotations { get; set; } = false;

        public bool OnlyExactMatchForAnnonations { get; set; } = true;
    }

    public class TSqlQueryConfiguration
    {
        public bool Active { get; set; } = false;

        public bool QuoteIdentifiers { get; set; } = true;
    }
}
