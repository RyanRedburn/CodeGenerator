namespace CodeGenerator.Model
{
    /// <summary>
    /// A class used to represent a database column for the purpose of generating code files.
    /// </summary>
    public class ColumnSpecification
    {
        public int ObjectId { get; set; }

        public string ColumnName { get; set; }

        public int ColumnType { get; set; }

        public bool IsComputed { get; set; }

        public bool IsNullable { get; set; }

        public bool IsIdentity { get; set; }
    }
}
