﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vulcan.Core.DataAccess.Entities;
using Vulcan.Core.DataAccess.Models;

namespace Vulcan.Core.DataAccess
{
    public class DynamicDataContext : IDisposable, IDataContext
    {
        private readonly IDbConnection _connection;
        public IDbConnection Connection => _connection;
        public string TenantId => _tenantId;

        private readonly string _tenantId;
        private readonly string _schemaName;

        public DynamicDataContext(string connectionStringName, string tenantId)
        {
            var connectionString = (connectionStringName == null ?
                ConfigurationManager.ConnectionStrings[0] :
                ConfigurationManager.ConnectionStrings[connectionStringName]).ConnectionString;
            _connection = new SqlConnection(connectionString);
            _tenantId = tenantId;
            _schemaName = $"{_tenantId}_core";
        }

        #region Query
        public IEnumerable<T> QueryDynamic<T>(string sql, object param = null, IDbTransaction transaction = null,
            bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : DynamicEntity, new()
        {
            var enumerable = _connection.Query(sql, param, transaction, buffered, commandTimeout, commandType);
            return ProcessDynamicData<T>(enumerable);
        }

        public async Task<IEnumerable<T>> QueryDynamicAsync<T>(string sql, object param = null, IDbTransaction transaction = null,
            int? commandTimeout = null, CommandType? commandType = null) where T : DynamicEntity, new()
        {
            var data = await _connection.QueryAsync<dynamic>(sql, param, transaction, commandTimeout, commandType);

            return ProcessDynamicData<T>(data);
        }
        #endregion

        #region Helpers
        public async Task<int> InsertAsync<T>(string schama, string tableName, T entity, List<Field> fields) where T : DynamicEntity
        {
            return await InsertAsync(schama, tableName, entity, fields, false);
        }

        internal async Task<int> InsertAsync<T>(string schama, string tableName, T entity, List<Field> fields, bool enableIdentityInsert) where T : DynamicEntity
        {
            var fieldNames = string.Join(",", fields.Where(f => !enableIdentityInsert && !f.IsAutoGenerated).Select(f => f.Name));
            var fieldParams = string.Join(",", fields.Where(f => !enableIdentityInsert && !f.IsAutoGenerated).Select(f => "@" + f.Name));

            var query =
                $"{(enableIdentityInsert ? $"SET IDENTITY_INSERT {schama}.{tableName} ON\n" : "")}" +
                $"INSERT INTO {schama}.{tableName}({fieldNames}) VALUES ({fieldParams})\n" +
                $"{(enableIdentityInsert ? $"SET IDENTITY_INSERT {schama}.{tableName} OFF" : "SELECT SCOPE_IDENTITY()")}";
            var dbArgs = new DynamicParameters();
            foreach (var pair in entity.EntityData) dbArgs.Add(pair.Key, pair.Value);
            var id = await this.ExecuteScalarAsync(schama, query, dbArgs);
            return int.Parse(id.ToString());
        }

        public async Task<int> UpdateAsync<T>(string schama, string tableName, int id, T entity, List<Field> fields) where T : DynamicEntity
        {
            var fieldParams = string.Join(",", fields.Where(f => !f.IsAutoGenerated).Select(f => f.Name + "=" + "@" + f.Name));

            var query = $"UPDATE {schama}.{tableName} SET {fieldParams} WHERE ID={id}";

            var dbArgs = new DynamicParameters();
            foreach (var pair in entity.EntityData) dbArgs.Add(pair.Key, pair.Value);

            return await this.ExecuteAsync(schama, query, dbArgs);
        }

        public async Task<int> DeleteAsync(string schama, string tableName, int id)
        {
            var query = $"DELETE FROM {schama}.{tableName} WHERE ID={id}";
            return await this.ExecuteAsync(schama, query);
        }
        #endregion

        private IEnumerable<T> ProcessDynamicData<T>(IEnumerable<dynamic> enumerable) where T : DynamicEntity, new()
        {
            return enumerable.Select(e => DynamicEntity.Parse<T>(e) as T);
        }

        public virtual void Dispose()
        {
            _connection.Dispose();
        }
    }
}
