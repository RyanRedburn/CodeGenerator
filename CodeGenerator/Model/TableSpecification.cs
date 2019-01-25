using CodeGenerator.Enumeration;
using System.Collections.Generic;

namespace CodeGenerator.Model
{
    /// <summary>
    /// A class used to represent a database table for the purpose of generating code files.
    /// </summary>
    public class TableSpecification
    {
        public int ObjectId { get; set; }

        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public List<ColumnSpecification> ColumnSpecifications { get; set; } = new List<ColumnSpecification>();

        public SpecificationProvider TableOrigin { get; set; }
    }
}
