using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Vulcan.Core.DataAccess
{
    public static class DataContextExtensions
    {
        public static string GetSchemaName(this IDataContext cnn, string schemaBaseName)
        {
            return $"[{cnn.TenantId}_{schemaBaseName.TrimStart('[').TrimEnd(']')}]";
        }

        private static string GetSql(IDataContext cnn, string schemaBaseName, string sql, CommandType? commandType = default(CommandType?))
        {
            if (schemaBaseName != null && commandType != null && commandType == CommandType.StoredProcedure)
            {
                sql = $"[{cnn.TenantId}_{schemaBaseName.TrimStart('[').TrimEnd(']')}].{sql}";
            }

            return sql;
        }
        
        #region Execute
        public static int Execute(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.Execute(sql, param, transaction, commandTimeout, commandType);
        }
        public static int Execute(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.Execute(command);
        }
        public static Task<int> ExecuteAsync(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }
        public static Task<int> ExecuteAsync(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteAsync(command);
        }
        #endregion

        #region ExecuteReader
        public static IDataReader ExecuteReader(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteReader(sql, param, transaction, commandTimeout, commandType);
        }
        public static IDataReader ExecuteReader(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteReader(command);
        }
        public static IDataReader ExecuteReader(this IDataContext cnn, CommandDefinition command, CommandBehavior commandBehavior)
        {
            return cnn.Connection.ExecuteReader(command, commandBehavior);
        }
        public static Task<IDataReader> ExecuteReaderAsync(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteReaderAsync(sql, param, transaction, commandTimeout, commandType);
        }
        public static Task<IDataReader> ExecuteReaderAsync(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteReaderAsync(command);
        }

        #endregion

        #region ExecuteScalar
        public static object ExecuteScalar(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
        }
        public static T ExecuteScalar<T>(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
        }
        public static object ExecuteScalar(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteScalar(command);
        }
        public static T ExecuteScalar<T>(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteScalar<T>(command);
        }
        public static Task<object> ExecuteScalarAsync(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, commandType);
        }
        public static Task<T> ExecuteScalarAsync<T>(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }
        public static Task<object> ExecuteScalarAsync(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteScalarAsync(command);
        }
        public static Task<T> ExecuteScalarAsync<T>(this IDataContext cnn, CommandDefinition command)
        {
            return cnn.Connection.ExecuteScalarAsync<T>(command);
        }
        #endregion

        #region Query
        public static IEnumerable<T> Query<T>(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);
            return cnn.Connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(this IDataContext cnn, string schemaBaseName, string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null)
        {
            sql = GetSql(cnn, schemaBaseName, sql, commandType);

            return await cnn.Connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }
        #endregion
    }
}
