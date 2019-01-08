using CodeGenerator.Enumeration;
using CodeGenerator.Helper;
using CodeGenerator.Interface.Repository;
using CodeGenerator.Model;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CodeGenerator.Repository
{
    /// <summary>
    /// Specification repository for SQL Server.
    /// </summary>
    public class SqlServerSpecificationRepository : ISpecificationRepository
    {
        private readonly IDbConnection _conn;

        public SqlServerSpecificationRepository(IDbConnection connection)
        {
            _conn = connection;
        }

        /// <summary>
        /// Gets all table specifications for the repository data source.
        /// </summary>
        /// <returns>IEnumerable of TableSpecification.</returns>
        public IEnumerable<TableSpecification> GetAll()
        {
            var tables = _conn.Query<TableSpecification>(QueryLibrary.MSSQL.GetTableSpecification).AsList();
            var columns = _conn.Query<ColumnSpecification>(QueryLibrary.MSSQL.GetColumnSpecification).AsList();

            tables.ForEach(t =>
            {
                t.ColumnSpecifications.AddRange(columns.Where(c => c.ObjectId == t.ObjectId));
                t.TableOrigin = SpecificationProvider.SqlServer;
            });

            return tables;
        }
    }
}
