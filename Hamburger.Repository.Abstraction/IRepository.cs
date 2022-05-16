using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamburger.Repository.Abstraction
{
    public interface IRepository<T> where T : class, new()
    {
        /// <summary>
        /// Insert entity into table.
        /// </summary>
        /// <param name="entity">An entity which is inserted as a row into table.</param>
        /// <returns>The added entity along with It's key.</returns>
        Task<T> Add(T entity);

        /// <summary>
        /// Insert many entities into table.
        /// </summary>
        /// <param name="entities">List of entities which are inserted as rows into table.</param>
        Task AddMany(IEnumerable<T> entities);

        /// <summary>
        /// Execute a command asynchronously using Task.
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <returns>The number of rows affected.</returns>
        Task<int> ExecuteQuery(string sql, object? param = null);

        /// <summary>
        /// Execute a command in a transaction, if an error happen before the command is finished, then changes won't be saved.
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        Task ExecuteQueryWithTransaction(string sql, object? param = null);

        /// <summary>
        /// Execute parameterized SQL that selects a single value.
        /// </summary>
        /// <typeparam name="U">The type to return.</typeparam>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="param">The parameters to use for this command.</param>
        /// <returns>The first cell returned, as U.</returns>
        Task<U> ExecuteScalar<U>(string sql, object? param = null);

        /// <summary>
        /// Get all records of the table.
        /// </summary>
        /// <returns>All records of the table.</returns>
        Task<IEnumerable<T>> Get();

        /// <summary>
        /// Finds an entity with the given primary key values.
        /// </summary>
        /// <param name="compositeIds">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found, or null.</returns>
        Task<T> Get(params object[] compositeIds);

        /// <summary>
        /// Retrieve a list of entities by executing a query.
        /// </summary>
        /// <param name="sql">The SQL to execute for this query.</param>
        /// <param name="param">The parameters to use for this query.</param>
        /// <returns>A sequence of data of T.</returns>
        Task<IEnumerable<T>> Get(string sql, object? param = null);

        /// <summary>
        /// Retrieve data by executing a query.
        /// </summary>
        /// <typeparam name="U">The type of results to return.</typeparam>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        Task<IEnumerable<U>> Get<U>(string sql, object? param = null);

        /// <summary>
        /// Delete an entity from table.
        /// </summary>
        /// <param name="compositeIds">The values of the primary key for the entity to be found.</param>
        Task Remove(params object[] compositeIds);

        /// <summary>
        /// Remove many entities from table.
        /// </summary>
        /// <param name="ids">List of keys whose entities will be deleted.</param>
        Task RemoveMany(IEnumerable<object> ids);

        /// <summary>
        /// Remove many entities from table which has more than 1 primary key.
        /// </summary>
        /// <param name="listCompositeIds">List of composite keys whose entities will be deleted.</param>
        Task RemoveManyCompositeKeys<U>(IEnumerable<IEnumerable<U>> listCompositeIds);

        /// <summary>
        /// Update an entity by It's key(s).
        /// </summary>
        /// <param name="entity">An entity to be updated.</param>
        Task Update(T entity);

        /// <summary>
        /// Update many entities by their keys.
        /// </summary>
        /// <param name="entities">Entities to be updated.</param>
        Task UpdateMany(IEnumerable<T> entities);

        /// <summary>
        /// Execute store procedure.
        /// </summary>
        /// <typeparam name="U">The type of results to return.</typeparam>
        /// <param name="storedProcedure">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <returns>A object of T.</returns>
        Task<U> ExecuteStoredProcedure<U>(string storedProcedure, object? param = null);
    }
}
