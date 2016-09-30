using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.Core.DataAccess.Entities;
using Vulcan.Core.DataAccess.Exceptions;
using Vulcan.Core.DataAccess.Models;
using Vulcan.Core.DataAccess.Validations;

namespace Vulcan.Core.DataAccess
{
    public abstract class LogicBase<T> : IDisposable where T : DynamicEntity, new()
    {
        public string SchemaName { get; }
        public string TableName { get; }
        public DynamicDataContext DynamicDataContext { get; }
        public DataDefinitionContext DataDefinitionContext { get; }
        public List<Field> Fields { get; private set; }
        public List<IValidator> Validators { get; set; }

        #region Constructors
        
        protected LogicBase(string schemaBaseName, string tableName, DynamicDataContext dynamicDataContext, DataDefinitionContext dataDefinitionContext)
        {
            SchemaName = dynamicDataContext.GetSchemaName(schemaBaseName);
            DataDefinitionContext = dataDefinitionContext;
            TableName = tableName;
            DynamicDataContext = dynamicDataContext;
            Fields = dataDefinitionContext.GetFields(tableName);
        }

        #endregion

        #region Data manipulations

        public async Task<int> GetCount()
        {
            var query = $"SELECT count(*) FROM {SchemaName}.{TableName}";
            var count = await DynamicDataContext.ExecuteScalarAsync(SchemaName, query);
            return int.Parse(count.ToString());
        }

        public async Task<IEnumerable<T>> GetAllAsync(int skip = 0, int take = 10)
        {
            var query =
                $"SELECT {string.Join(",", Fields.Select(f => f.Name))} FROM {SchemaName}.{TableName} ORDER BY ID OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
            var leads = await DynamicDataContext.QueryDynamicAsync<T>(query);
            return leads;
        }

        public async Task<T> GetAsync(int id)
        {
            var query = $"SELECT TOP 1 {string.Join(",", Fields.Select(f => f.Name))} FROM {TableName} WHERE Id={id}";
            var leads = await DynamicDataContext.QueryDynamicAsync<T>(query);
            return leads.FirstOrDefault();
        }

        public async Task<int> AddAsync(T entity)
        {
            return await DynamicDataContext.InsertAsync(SchemaName, TableName, entity, Fields);
        }

        public async Task<int> UpdateAsync(int id, T entity)
        {
            return await DynamicDataContext.UpdateAsync(SchemaName, TableName, id, entity, Fields);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DynamicDataContext.DeleteAsync(SchemaName, TableName, id);
        }

        #endregion

        #region Field manipulations

        public int GetFieldCount(bool force = false)
        {
            if (force)
                Fields = DataDefinitionContext.GetFields(TableName, true);

            return Fields.Count;
        }

        /// <exception cref="ValidationErrorException">Invalid validation exception.</exception>
        public async Task<int> AddFieldAsync(Field field)
        {
            field.TableName = TableName;
            field.Validations.ForEach(v =>
            {
                v.FieldName = field.Name;
            });
            var id = await DataDefinitionContext.AddFieldAsync(field);
            Fields = DataDefinitionContext.GetFields(TableName, true);
            return id;
        }

        public IEnumerable<Field> GetFields(bool force = false, int skip = 0, int take = 10)
        {
            if (force)
                Fields = DataDefinitionContext.GetFields(TableName, true);

            return Fields.Skip(skip).Take(take);
        }

        public async Task<Field> GetFieldAsync(int id, bool force = false)
        {
            return await DataDefinitionContext.GetFieldAsync(id, force);
        }

        /// <exception cref="FieldNotExistsException">Throws when field not exists.</exception>
        public async Task<int> DeleteFieldAsync(int id)
        {

            if (Fields.All(f => f.Id != id))
            {
                throw new FieldNotExistsException();
            }

            await DataDefinitionContext.DeleteFieldAsync(id);
            Fields = DataDefinitionContext.GetFields(TableName, true);
            return id;
        }

        public async Task<int> UpdateFieldAsync(int id, Field field)
        {
            await DataDefinitionContext.UpdateFieldAsync(id, field);
            Fields = DataDefinitionContext.GetFields(TableName, true);

            return id;
        }

        #endregion

        #region Field validation manipulations

        public async Task<int> AddFieldValidationAsync(int fieldId, FieldValidation validation)
        {
            var id = await DataDefinitionContext.AddValidationAsync(fieldId, validation);
            Fields = DataDefinitionContext.GetFields(TableName, true);
            return id;
        }

        public async Task<int> UpdateFieldValidationAsync(int fieldId, int validationId, FieldValidation validation)
        {
            var id = await DataDefinitionContext.UpdateValidationAsync(fieldId, validationId, validation);
            Fields = DataDefinitionContext.GetFields(TableName, true);
            return id;
        }

        public async Task<List<FieldValidation>> GetFieldValidationsAsync(int fieldId, bool force)
        {
            return await DataDefinitionContext.GetValidationsAsync(fieldId, force);
        }

        public async Task<int> DeleteFieldValidationAsync(int validationId)
        {
            var id = await DataDefinitionContext.DeleteValidationAsync(validationId);
            Fields = DataDefinitionContext.GetFields(TableName, true);
            return id;
        }

        #endregion

        #region Validation providers
        public bool Validate(T entity)
        {
            LoadValidators();

            var provider = new ValidationProvider();
            return provider.Validate(entity, Validators);
        }

        public void LoadValidators()
        {
            if (this.Validators != null) return;

            this.Validators = new List<IValidator>();
            foreach (var field in this.Fields)
            {
                this.Validators.AddRange(field.Validations.Select(v => v.Validator));
            }
        }

        #endregion

        public virtual void Dispose()
        {
            DataDefinitionContext.Dispose();
            DynamicDataContext.Dispose();
        }
    }
}
