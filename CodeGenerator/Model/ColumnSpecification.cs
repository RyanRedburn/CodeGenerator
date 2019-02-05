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

        public short ColumnLength { get; set; }

        public bool IsComputed { get; set; } = false;

        public bool IsNullable { get; set; } = true;

        public bool IsIdentity { get; set; } = false;
    }
}
