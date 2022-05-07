using Dapper;
using Hamburger.Helpers;
using Hamburger.Helpers.Extensions;
using Hamburger.Repository.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Hamburger.Repository.EF
{
    public abstract class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly ApplicationDbContext _context;
        protected readonly string _tableName;
        protected readonly string _primaryKey;
        protected readonly PropertyInfo _primaryKeyProperty;
        protected readonly IList<string> _primaryKeys;
        protected readonly IList<PropertyInfo> _primaryKeyProperties;
        public readonly bool IsCompositeKey;

        public Repository(ApplicationDbContext context, string primaryKey)
        {
            _context = context;
            _primaryKey = primaryKey;
            _primaryKeyProperty = typeof(T).GetProperty(primaryKey);
            _primaryKeys = new List<string> { primaryKey };
            _primaryKeyProperties = new List<PropertyInfo> { _primaryKeyProperty };
            IsCompositeKey = false;

            var entityType = _context.Model.FindEntityType(typeof(T));
            _tableName = entityType.GetTableName();
        }

        public Repository(ApplicationDbContext context, IEnumerable<string> primaryKeys)
        {
            _context = context;
            _primaryKeys = primaryKeys.ToList();
            _primaryKeyProperties = primaryKeys.Select(key => typeof(T).GetProperty(key)).ToList();
            _primaryKey = primaryKeys.First();
            _primaryKeyProperty = _primaryKeyProperties.First();
            IsCompositeKey = primaryKeys.Count() > 1;

            var entityType = _context.Model.FindEntityType(typeof(T));
            _tableName = entityType.GetTableName();
        }

        public async Task<T> Add(T entity)
        {
            var result = await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task AddMany(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<int> ExecuteQuery(string sql, object? param = null)
        {
            int result;

            using (var connection = _context.Database.GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.ExecuteAsync(sql, param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        public async Task ExecuteQueryWithTransaction(string sql, object? param = null)
        {
            using (var connection = _context.Database.GetDbConnection())
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

        public async Task<U> ExecuteScalar<U>(string sql, object? param = null)
        {
            U result;

            using (var connection = _context.Database.GetDbConnection())
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

        public async Task<U> ExecuteStoredProcedure<U>(string storedProcedure, object? param = null)
        {
            IEnumerable<object> result;

            using (var connection = _context.Database.GetDbConnection())
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

        public async Task<IEnumerable<T>> Get()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> Get(params object[] compositeIds)
        {
            if (compositeIds.IsNullOrEmpty())
                throw CustomException.Validation.PropertyIsNullOrEmpty(nameof(compositeIds));

            return await _context.Set<T>().FindAsync(compositeIds);
        }

        public async Task<IEnumerable<T>> Get(string sql, object? param = null)
        {
            return await Get<T>(sql, param);
        }

        public async Task<IEnumerable<U>> Get<U>(string sql, object? param = null)
        {
            IEnumerable<U> result;

            using (var connection = _context.Database.GetDbConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                result = await connection.QueryAsync<U>(sql, param);

                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }

        protected void SetKeysForEntity(T entity, IEnumerable<object> keyValues)
        {
            if (_primaryKeyProperties.Count() > keyValues.Count())
                throw CustomException.Database.NotProvidedEnoughValues;

            var valuesList = keyValues.ToList();

            for (int i = 0; i < _primaryKeyProperties.Count; i++)
            {
                _primaryKeyProperties[i].SetValue(entity, valuesList[i]);
            }
        }

        public async Task Remove(params object[] compositeIds)
        {
            var entity = new T();

            SetKeysForEntity(entity, compositeIds);

            _context.Attach(entity);
            _context.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveMany(IEnumerable<object> ids)
        {
            var entities = ids.Select(id =>
            {
                var entity = new T();
                _primaryKeyProperty.SetValue(entity, id);

                _context.Attach(entity);
                _context.Remove(entity);

                return entity;
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveMany(IEnumerable<IEnumerable<object>> listCompositeIds)
        {
            var entities = listCompositeIds.Select(compositeIds =>
            {
                var entity = new T();
                SetKeysForEntity(entity, compositeIds);

                _context.Attach(entity);
                _context.Remove(entity);

                return entity;
            });

            await _context.SaveChangesAsync();
        }

        public async Task Update(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMany(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
            await _context.SaveChangesAsync();
        }
    }
}
