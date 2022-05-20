using Dapper;
using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Models.Entities.Abstractions;
using Hamburger.Repository.Abstraction;
using Hamburger.Repository.Dapper.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hamburger.Repository.Dapper
{
    public abstract class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly string _tableName;
        protected readonly string _primaryKey;
        protected readonly PropertyInfo _primaryKeyProperty;
        protected readonly IList<string> _primaryKeys;
        protected readonly IList<PropertyInfo> _primaryKeyProperties;
        public readonly bool IsCompositeKey;

        public Repository(string tableName, string primaryKey)
        {
            _tableName = tableName;
            _primaryKey = primaryKey;
            _primaryKeyProperty = typeof(T).GetProperty(primaryKey);
            _primaryKeys = new List<string> { primaryKey };
            _primaryKeyProperties = new List<PropertyInfo> { _primaryKeyProperty };
            IsCompositeKey = false;
        }

        public Repository(string tableName, IEnumerable<string> primaryKeys)
        {
            _tableName = tableName;
            _primaryKeys = primaryKeys.ToList();
            _primaryKeyProperties = primaryKeys.Select(key => typeof(T).GetProperty(key)).ToList();
            _primaryKey = primaryKeys.First();
            _primaryKeyProperty = _primaryKeyProperties.First();
            IsCompositeKey = primaryKeys.Count() > 1;
        }

        protected DbConnection GetDbConnection() => new SqlConnection(AppSettings.Database.ConnectionString);

        public virtual async Task<T> Add(T entity)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (entity is IEntityDate entityDate)
                {
                    entityDate.CreatedDate = DateTime.UtcNow;
                    entityDate.ModifiedDate = DateTime.UtcNow;
                }

                var param = DynamicParametersHelper.BuildDynamicParameters(entity);

                // if primary key type is short, int or long
                // then we assume it's auto increased
                // and perform insert without primary key field
                if (!IsCompositeKey && (_primaryKeyProperty.PropertyType == typeof(short) || _primaryKeyProperty.PropertyType == typeof(int) || _primaryKeyProperty.PropertyType == typeof(long)))
                {
                    var id = await connection.Insert<object>(_tableName, _primaryKey, param);

                    _primaryKeyProperty.SetValue(entity, id);
                }
                else
                {
                    await connection.InsertWithPrimaryKey(_tableName, param);
                }

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return entity;
        }

        public virtual async Task AddMany(IEnumerable<T> entities)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(IEntityDate).IsAssignableFrom(typeof(T)))
                {
                    foreach (IEntityDate entity in entities)
                    {
                        entity.CreatedDate = DateTime.UtcNow;
                        entity.ModifiedDate = DateTime.UtcNow;
                    }
                }

                var paramList = entities.Select(DynamicParametersHelper.BuildDynamicParameters);

                // if primary key type is short, int or long
                // then we assume it's auto increased
                // and perform insert without primary key field
                if (!IsCompositeKey && (_primaryKeyProperty.PropertyType == typeof(short) || _primaryKeyProperty.PropertyType == typeof(int) || _primaryKeyProperty.PropertyType == typeof(long)))
                {
                    await connection.BulkInsert(_tableName, _primaryKey, paramList);
                }
                else
                {
                    await connection.BulkInsertWIthPrimaryKey(_tableName, paramList);
                }

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task<int> ExecuteQuery(string sql, object param = null)
        {
            int result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.ExecuteAsync(sql, param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public virtual async Task ExecuteQueryWithTransaction(string sql, object param = null)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await connection.ExecuteAsync(sql, param);
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task<U> ExecuteScalar<U>(string sql, object param = null)
        {
            U result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.ExecuteScalarAsync<U>(sql, param);

                if (result == null)
                    throw CustomException.Database.ExecuteScalarReturnsEmpty;

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public virtual async Task<U> ExecuteStoredProcedure<U>(string storedProcedure, object param = null)
        {
            IEnumerable<object> result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.QueryAsync<object>(storedProcedure, param, commandType: CommandType.StoredProcedure);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(U)))
                return (U)result;
            return (U)result.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<T>> Get()
        {
            IEnumerable<T> result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    result = await connection.GetAllForSoftDelete<T>(_tableName);
                }
                else result = await connection.GetAll<T>(_tableName);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public virtual async Task<T> Get(params object[] compositeIds)
        {
            if (compositeIds.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(compositeIds));

            T result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    result = await connection.GetByIdForSoftDelete<T>(_tableName, _primaryKeys, compositeIds);
                }
                else result = await connection.GetById<T>(_tableName, _primaryKeys, compositeIds);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public virtual async Task<IEnumerable<T>> Get(string sql, object param = null)
        {
            return await Get<T>(sql, param);
        }

        public virtual async Task<IEnumerable<U>> Get<U>(string sql, object param = null)
        {
            IEnumerable<U> result;

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.QueryAsync<U>(sql, param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public virtual async Task Remove(params object[] compositeIds)
        {
            if (compositeIds.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(compositeIds));

            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    await connection.SoftDelete(_tableName, _primaryKeys, compositeIds);
                }
                else await connection.Delete(_tableName, _primaryKeys, compositeIds);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task RemoveMany(IEnumerable<object> ids)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    await connection.BulkSoftDelete(_tableName, _primaryKey, ids);
                }
                else await connection.BulkDelete(_tableName, _primaryKey, ids);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task RemoveManyCompositeKeys<U>(IEnumerable<IEnumerable<U>> listCompositeIds)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                var listCompositeIds2 = listCompositeIds.Select(ids => ids.Cast<object>());

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
                {
                    await connection.BulkSoftDelete(_tableName, _primaryKeys, listCompositeIds2);
                }
                else await connection.BulkDelete(_tableName, _primaryKeys, listCompositeIds2);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task Update(T entity)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (entity is IEntityDate entityDate)
                {
                    entityDate.ModifiedDate = DateTime.UtcNow;
                }

                var param = DynamicParametersHelper.BuildDynamicParameters(entity);

                await connection.Update(_tableName, _primaryKeys, param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }

        public virtual async Task UpdateMany(IEnumerable<T> entities)
        {
            using (var connection = GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                if (typeof(IEntityDate).IsAssignableFrom(typeof(T)))
                {
                    foreach (IEntityDate entity in entities)
                    {
                        entity.ModifiedDate = DateTime.UtcNow;
                    }
                }

                var paramList = entities.Select(DynamicParametersHelper.BuildDynamicParameters);

                await connection.BulkUpdate(_tableName, _primaryKeys, paramList);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }
    }
}
