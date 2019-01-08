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

        public string SourceConnection { get; set; }

        public ConnectionType ConnectionType { get; set; }

        public CSharpConfiguration CSharpConfiguration { get; set; } = new CSharpConfiguration();

        public TSqlConfiguration TSqlConfiguration { get; set; } = new TSqlConfiguration();

        #endregion
    }

    public class CSharpConfiguration
    {
        public bool Active { get; set; } = false;

        public string ModelNameSpace { get; set; }

        public bool RequestNameSpaceOnExec { get; set; } = false;
    }

    public class TSqlConfiguration
    {
        public bool Active { get; set; } = false;

        public bool QuoteIdentifiers { get; set; } = true;
    }
}
