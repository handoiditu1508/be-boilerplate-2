using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hamburger.Repository.Dapper.Helpers
{
    internal static class DynamicQuery
    {
        /// <summary>
        /// Get insert query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Table's primary key to be excluded from query.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>INSERT INTO Table (Col1,Col2) VALUES (@Col1,@Col2);</returns>
        public static string GetInsertQuery(string tableName, string primaryKey, DynamicParameters param)
        {
            var columns = param.ParameterNames.Where(s => s != primaryKey);


            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2});",
                            tableName,
                            string.Join(',', columns),
                            string.Join(",@", columns));
            return sql;
        }

        /// <summary>
        /// Get insert query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>INSERT INTO Table (Key1,Col1,Col2) VALUES (@Key1,@Col1,@Col2);</returns>
        public static string GetInsertQueryWithPrimaryKey(string tableName, DynamicParameters param)
        {
            var columns = param.ParameterNames;


            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2});",
                            tableName,
                            string.Join(',', columns),
                            string.Join(",@", columns));
            return sql;
        }

        /// <summary>
        /// Get update query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKeys">Table's primary keys to be excluded from query and use in conditional phrase.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>UPDATE Table SET Col1 = @Col1,Col2 = @Col2 WHERE Key1 = @Key1 AND Key2 = @Key2;</returns>
        public static string GetUpdateQuery(string tableName, IEnumerable<string> primaryKeys, DynamicParameters param)
        {
            var parameters = param.ParameterNames.Where(s => !primaryKeys.Contains(s)).Select(s => $"{s} = @{s}");
            var keyParameters = primaryKeys.Select(s => $"{s} = @{s}");

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2};",
                            tableName,
                            string.Join(',', parameters),
                            string.Join(" AND ", keyParameters));
            return sql;
        }

        /// <summary>
        /// Get update query for multiple records.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Table's primary key to be excluded from query and use in conditional phrase.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>UPDATE Table SET Col1 = @Col1,Col2 = @Col2 WHERE Key1 IN @Key1;</returns>
        public static string GetBulkUpdateWithSimilarParametersQuery(string tableName, string primaryKey, DynamicParameters param)
        {
            var parameters = param.ParameterNames.Where(s => s != primaryKey).Select(s => $"{s} = @{s}");

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2};",
                            tableName,
                            string.Join(',', parameters),
                            $"{primaryKey} IN @{primaryKey}");
            return sql;
        }

        /// <summary>
        /// Get query that retrieve the last inserted Id.
        /// </summary>
        /// <returns>SELECT SCOPE_IDENTITY();</returns>
        public static string GetLastInsertedIdQuery() => "SELECT SCOPE_IDENTITY();";

        /// <summary>
        /// Get query that select all from table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>SELECT * FROM Table;</returns>
        public static string GetSelectAllQuery(string tableName) => $"SELECT * FROM {tableName};";

        /// <summary>
        /// Get query that select from table with conditional phrase.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>SELECT * FROM Table WHERE Col1 = @Col1 AND Col2 = @Col2;</returns>
        public static string GetSelectByParametersQuery(string tableName, DynamicParameters param)
        {
            var parameters = param.ParameterNames.Select(s => $"{s} = @{s}");

            string sql = string.Format("SELECT * FROM {0} WHERE {1};",
                            tableName,
                            string.Join(" AND ", parameters));
            return sql;
        }

        /// <summary>
        /// Get delete query with conditional phrase.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="param">Dynamic object with properties is column names.</param>
        /// <returns>DELETE FROM Table WHERE Col1 = @Col1 AND Col2 = @Col2;</returns>
        public static string GetDeleteByParametersQuery(string tableName, DynamicParameters param)
        {
            var parameters = param.ParameterNames.Select(s => $"{s} = @{s}");

            string sql = string.Format("DELETE FROM {0} WHERE {1};",
                            tableName,
                            string.Join(" AND ", parameters));
            return sql;
        }

        /// <summary>
        /// Get delete query for multiple records.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="primaryKey">Table's primary key to use in conditional phrase.</param>
        /// <returns>DELETE FROM Table WHERE Key1 IN @Key1;</returns>
        public static string GetBulkDeleteQuery(string tableName, string primaryKey) => $"DELETE FROM {tableName} WHERE {primaryKey} IN @{primaryKey};";

        /// <summary>
        /// Get query function that extract Date from DateTime.
        /// </summary>
        /// <param name="dateTime">DateTime string.</param>
        /// <returns>DATEADD(dd, 0, DATEDIFF(dd, 0, EntityDate))</returns>
        public static string GetDateFromDateTimeFunction(string dateTime) => $"DATEADD(dd, 0, DATEDIFF(dd, 0, {dateTime}))";
    }
}
