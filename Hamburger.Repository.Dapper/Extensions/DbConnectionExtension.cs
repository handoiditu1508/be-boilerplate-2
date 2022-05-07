using Dapper;
using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Entities.Abstractions;
using Hamburger.Repository.Dapper.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hamburger.Repository.Dapper.Extensions
{
    internal static class DbConnectionExtension
    {
        /// <summary>
        /// Insert with primary key.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        public static async Task InsertWithPrimaryKey(this IDbConnection cnn, string tableName, dynamic param)
        {
            if (param == null)
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetInsertQueryWithPrimaryKey(tableName, param);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Insert with primary key and return inserted Id.
        /// </summary>
        /// <typeparam name="T">Id data type.</typeparam>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Name of the primary key.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>Inserted Id.</returns>
        public static async Task<T> Insert<T>(this IDbConnection cnn, string tableName, string primaryKey, dynamic param)
        {
            if (param == null)
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetInsertQuery(tableName, primaryKey, param);
            sql += DynamicQuery.GetLastInsertedIdQuery();

            return await SqlMapper.ExecuteScalarAsync<T>(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Update with param.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        public static async Task Update(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, dynamic param)
        {
            if (param == null)
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetUpdateQuery(tableName, primaryKeys, param);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Insert many records excluding primary key.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Name of the primary key.</param>
        /// <param name="paramList">Dynamic object with properties is column names.</param>
        public static async Task BulkInsert(this IDbConnection cnn, string tableName, string primaryKey, IEnumerable<dynamic> paramList)
        {
            if (paramList.IsNullOrEmpty())
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetInsertQuery(tableName, primaryKey, paramList.First());

            await SqlMapper.ExecuteAsync(cnn, sql, paramList, commandTimeout: 120);
        }

        /// <summary>
        /// Insert many records including primary key.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="paramList">Dynamic object with properties is column names.</param>
        public static async Task BulkInsertWIthPrimaryKey(this IDbConnection cnn, string tableName, IEnumerable<dynamic> paramList)
        {
            if (paramList.IsNullOrEmpty())
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetInsertQueryWithPrimaryKey(tableName, paramList.First());

            await SqlMapper.ExecuteAsync(cnn, sql, paramList, commandTimeout: 120);
        }

        /// <summary>
        /// Update many records.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="paramList">Dynamic object with properties is column names.</param>
        public static async Task BulkUpdate(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<dynamic> paramList)
        {
            if (paramList.IsNullOrEmpty())
                throw CustomException.Database.ParamIsEmpty;

            var sql = DynamicQuery.GetUpdateQuery(tableName, primaryKeys, paramList.First());

            await SqlMapper.ExecuteAsync(cnn, sql, paramList, commandTimeout: 120);
        }

        /// <summary>
        /// Get all from table and cast to type T.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>All records in table.</returns>
        public static async Task<IEnumerable<T>> GetAll<T>(this IDbConnection cnn, string tableName)
        {
            var sql = DynamicQuery.GetSelectAllQuery(tableName);
            return await SqlMapper.QueryAsync<T>(cnn, sql, null, commandTimeout: 120);
        }

        /// <summary>
        /// Get all from table where ISoftDelete.IsDeleted = false and cast to type T.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>All records in table.</returns>
        public static async Task<IEnumerable<T>> GetAllForSoftDelete<T>(this IDbConnection cnn, string tableName)
        {
            var param = new DynamicParameters();
            param.Add(nameof(ISoftDelete.IsDeleted), false, direction: ParameterDirection.Input);

            var sql = DynamicQuery.GetSelectByParametersQuery(tableName, param);
            return await SqlMapper.QueryAsync<T>(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Get by primary key.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="compositeIds">All primary key's value.</param>
        /// <returns>Object typed T.</returns>
        public static async Task<T> GetById<T>(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<object> compositeIds)
        {
            var param = DynamicParametersHelper.BuildDynamicParameters(primaryKeys, compositeIds);

            var sql = DynamicQuery.GetSelectByParametersQuery(tableName, param);

            var result = await SqlMapper.QueryAsync<T>(cnn, sql, param, commandTimeout: 120);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by primary key where ISoftDelete.IsDeleted = false.
        /// </summary>
        /// <typeparam name="T">Type to cast.</typeparam>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="compositeIds">All primary key's value.</param>
        /// <returns>Object typed T.</returns>
        public static async Task<T> GetByIdForSoftDelete<T>(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<object> compositeIds)
        {
            var param = DynamicParametersHelper.BuildDynamicParameters(primaryKeys, compositeIds);
            param.Add(nameof(ISoftDelete.IsDeleted), false, direction: ParameterDirection.Input);

            var sql = DynamicQuery.GetSelectByParametersQuery(tableName, param);

            var result = await SqlMapper.QueryAsync<T>(cnn, sql, param, commandTimeout: 120);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Delete a record.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="compositeIds">All primary key's value.</param>
        public static async Task Delete(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<object> compositeIds)
        {
            var param = DynamicParametersHelper.BuildDynamicParameters(primaryKeys, compositeIds);

            var sql = DynamicQuery.GetDeleteByParametersQuery(tableName, param);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Handle delete operation for ISoftDelete.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="compositeIds">All primary key's value.</param>
        public static async Task SoftDelete(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<object> compositeIds)
        {
            var param = DynamicParametersHelper.BuildDynamicParameters(primaryKeys, compositeIds);
            param.Add(nameof(ISoftDelete.IsDeleted), true, direction: ParameterDirection.Input);
            param.Add(nameof(ISoftDelete.DeletedDate), DateTime.UtcNow, direction: ParameterDirection.Input);

            var sql = DynamicQuery.GetUpdateQuery(tableName, primaryKeys, param);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Delete many records.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Name of the primary key.</param>
        /// <param name="ids">List of keys need to delete.</param>
        public static async Task BulkDelete(this IDbConnection cnn, string tableName, string primaryKey, IEnumerable<object> ids)
        {
            var sql = DynamicQuery.GetBulkDeleteQuery(tableName, primaryKey);

            var param = new DynamicParameters();
            param.Add(primaryKey, ids, direction: ParameterDirection.Input);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Handle delete many records for ISoftDelete.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Name of the primary key.</param>
        /// <param name="ids">List of keys need to delete.</param>
        public static async Task BulkSoftDelete(this IDbConnection cnn, string tableName, string primaryKey, IEnumerable<object> ids)
        {
            var param = new DynamicParameters();
            param.Add(primaryKey, ids, direction: ParameterDirection.Input);
            param.Add(nameof(ISoftDelete.IsDeleted), true, direction: ParameterDirection.Input);
            param.Add(nameof(ISoftDelete.DeletedDate), DateTime.UtcNow, direction: ParameterDirection.Input);

            var sql = DynamicQuery.GetBulkUpdateWithSimilarParametersQuery(tableName, primaryKey, param);

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Delete many records for table with composite keys.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="listCompositeIds">List of composite keys need to delete.</param>
        public static async Task BulkDelete(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<IEnumerable<object>> listCompositeIds)
        {
            var param = listCompositeIds.Select(c => DynamicParametersHelper.BuildDynamicParameters(primaryKeys, c));

            var sql = DynamicQuery.GetDeleteByParametersQuery(tableName, param.First());

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }

        /// <summary>
        /// Handle soft delete many records for table with composite keys.
        /// </summary>
        /// <param name="cnn">An IDbConnection instance.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">List of table's keys.</param>
        /// <param name="listCompositeIds">List of composite keys need to delete.</param>
        public static async Task BulkSoftDelete(this IDbConnection cnn, string tableName, IEnumerable<string> primaryKeys, IEnumerable<IEnumerable<object>> listCompositeIds)
        {
            var param = listCompositeIds.Select(c =>
            {
                var p = DynamicParametersHelper.BuildDynamicParameters(primaryKeys, c);
                p.Add(nameof(ISoftDelete.IsDeleted), true, direction: ParameterDirection.Input);
                p.Add(nameof(ISoftDelete.DeletedDate), DateTime.UtcNow, direction: ParameterDirection.Input);
                return p;
            });

            var sql = DynamicQuery.GetUpdateQuery(tableName, primaryKeys, param.First());

            await SqlMapper.ExecuteAsync(cnn, sql, param, commandTimeout: 120);
        }
    }
}
