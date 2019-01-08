using CodeGenerator.Model;
using System.Collections.Generic;

namespace CodeGenerator.Interface.Repository
{
    /// <summary>
    /// Repository used to retrieve model definitions from a given data source.
    /// </summary>
    public interface ISpecificationRepository
    {
        /// <summary>
        /// Gets all table specifications for the repository data source.
        /// </summary>
        /// <returns>IEnumerable of TableSpecification.</returns>
        IEnumerable<TableSpecification> GetAll();
    }
}
