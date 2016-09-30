using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Dapper;
using Vulcan.Core.DataAccess.Exceptions;
using Vulcan.Core.DataAccess.Models;
using Vulcan.Core.DataAccess.Validations.Validators;

namespace Vulcan.Core.DataAccess
{
    public class DataDefinitionContext : IDisposable, IDataContext
    {
        private IDbConnection _connection;
        private readonly string _connectionString;
        public IDbConnection Connection
        {
            get
            {
                LoadConnection();
                return _connection;
            }
        }
        public string TenantId => _tenantId;

        private readonly string _tenantId;
        private readonly string _internalSchemaName;

        #region Constructors
        public DataDefinitionContext(string connectionStringName, string tenantId)
        {
            _connectionString = (connectionStringName == null ?
                ConfigurationManager.ConnectionStrings[0] :
                ConfigurationManager.ConnectionStrings[connectionStringName]).ConnectionString;
            if (tenantId == null)
            {
                _internalSchemaName = $"core";
            }
            else
            {
                _tenantId = tenantId;
                _internalSchemaName = $"{_tenantId}_core";
            }
        }
        #endregion


        #region Field manipulations

        /// <exception cref="ValidationErrorException">Invalid validation exception.</exception>
        /// <exception cref="InvalidExpressionException">The expression is invalid. See the <see cref="P:System.Data.DataColumn.Expression" /> property for more information about how to create expressions. </exception>
        /// <exception cref="DuplicateNameException">The collection already has a column with the specified name. (The comparison is not case-sensitive.) </exception>
        /// <exception cref="ArgumentException">The array is larger than the number of columns in the table.</exception>
        /// <exception cref="NoNullAllowedException">Trying to put a null in a column where <see cref="P:System.Data.DataColumn.AllowDBNull" /> is false. </exception>
        /// <exception cref="ConstraintException">Adding the row invalidates a constraint. </exception>
        /// <exception cref="InvalidCastException">A value does not match its respective column type. </exception>
        public async Task<int> AddFieldAsync(Field field, bool fieldIdentityInsert = false, bool validationIdentityInsert = false, bool choicesIdentityInsert = false)
        {
            LoadConnection();

            var context = new ValidationContext(field, null, null);
            var errors = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(field, context, errors, true);

            if (!isValid)
            {
                throw new ValidationErrorException(errors);
            }

            var validations = new DataTable();
            validations.Columns.Add("Id", typeof(int));
            validations.Columns.Add("ValidatorId", typeof(Guid));
            validations.Columns.Add("Message", typeof(string));
            validations.Columns.Add("Data", typeof(string));

            switch (field.Type)
            {
                case FieldType.String:
                    break;
                case FieldType.Text:
                    break;
                case FieldType.Number:
                    field.Validations.Add(new FieldValidation(new NumberValidator())
                    {
                        FieldName = field.Name,
                        Message = "Invalid datatype"
                    });
                    break;
                case FieldType.DateTime:
                    field.Validations.Add(new FieldValidation(new DateTimeValidator())
                    {
                        FieldName = field.Name,
                        Message = "Invalid datatype"
                    });
                    break;
                case FieldType.Boolean:
                    field.Validations.Add(new FieldValidation(new BooleanValidator())
                    {
                        FieldName = field.Name,
                        Message = "Invalid datatype"
                    });
                    break;
                case FieldType.Choice:
                    field.Validations.Add(new FieldValidation(new ChoiceValidator()
                    {
                        Choices = field.Choices.Select(c => c.Text).ToList(),
                        FieldName = field.Name,
                        Message = "Invalid value"
                    }));
                    break;
            }

            foreach (var fieldValidation in field.Validations)
            {
                validations.Rows.Add(fieldValidation.Id, fieldValidation.ValidatorId, fieldValidation.Message, fieldValidation.Data);
            }

            var choices = new DataTable();
            choices.Columns.Add("Id", typeof (int));
            choices.Columns.Add("Text", typeof (string));

            if (field.Type == FieldType.Choice)
            {
                foreach (var fieldChoice in field.Choices)
                {
                    choices.Rows.Add(fieldChoice.Id, fieldChoice.Text);
                }
            }

            var id = await _connection.ExecuteScalarAsync<int>($"{_internalSchemaName}.[base_Field_Add]", new
            {
                TableName = field.TableName,
                Name = field.Name,
                Title = field.Title,
                Type = field.Type,
                IsHidden = field.IsHidden,
                IsDeleted = field.IsDeleted,
                IsSystem = field.IsSystem,
                // InternalName = field.InternalName,
                IsAutoGenerated = field.IsAutoGenerated,
                Validations = validations,
                Id = field.Id,
                FieldIdentityInsert = fieldIdentityInsert,
                ValidationIdentityInsert = validationIdentityInsert,
                ShowInUi = field.ShowInUi,
                UiIndex = field.UiIndex,
                Choices = choices,
                ChoiceIdentityInsert = choicesIdentityInsert
            }, commandType: CommandType.StoredProcedure);

            return id;
        }

        //public async Task<int> AddField(Field field)
        //{
        //    return await AddField(field, false, false);
        //}

        public async Task<Field> GetFieldAsync(int id, bool force = false)
        {
            if (!force) return GetFields().FirstOrDefault(f => f.Id == id);

            LoadConnection();

            var field = (await _connection.QueryAsync<Field>($"{_internalSchemaName}.[base_Field_GetById]", new
            {
                id = id
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
            if (field == null)
            {
                return null;
            }

            var validations = (_connection.Query<FieldValidation>($"{_internalSchemaName}.[base_FieldValidation_GetByFieldId]", new
            {
                fieldId = id
            }, commandType: CommandType.StoredProcedure)).ToList();

            field.Validations = validations;
            return field;
        }

        public List<Field> GetFields(string tableName = null, bool force = false)
        {
            var fieldsCache = MemoryCache.Default.Get("DataFields") as Field[];
            if (!force)
            {
                if (fieldsCache != null) return tableName == null ? fieldsCache.ToList() : fieldsCache.Where(f => f.TableName == tableName).ToList();
            }

            LoadConnection();

            fieldsCache = (_connection.Query<Field>($"{_internalSchemaName}.[base_Field_Get]", commandType: CommandType.StoredProcedure)).ToArray();
            var validations = (_connection.Query<FieldValidation>($"{_internalSchemaName}.[base_FieldValidation_Get]", commandType: CommandType.StoredProcedure)).ToList();
            var choices = (_connection.Query<FieldChoice>($"{_internalSchemaName}.[base_FieldChoice_Get]", commandType: CommandType.StoredProcedure)).ToList();

            foreach (var field in fieldsCache)
            {
                field.Validations = validations.Where(v => v.FieldId == field.Id).ToList();
                field.Choices = choices.Where(v => v.FieldId == field.Id).ToList();
            }

            var policy = new CacheItemPolicy
            {
                Priority = CacheItemPriority.NotRemovable, SlidingExpiration = TimeSpan.FromHours(24)
            };
            MemoryCache.Default.Set("DataFields", fieldsCache, policy);

            return tableName == null ? fieldsCache.ToList() : fieldsCache.Where(f => f.TableName == tableName).ToList();
        }

        public async Task<int> DeleteFieldAsync(int id)
        {
            LoadConnection();

            try
            {
                await _connection.ExecuteAsync($"{_internalSchemaName}.[base_Field_Delete]", new
                {
                    id = id
                }, commandType: CommandType.StoredProcedure);

                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <exception cref="FieldNotExistsException">Throws when field not exists.</exception>
        /// <exception cref="ValidationNotExistsException">Throws when validation not exists.</exception>
        public async Task<int> UpdateFieldAsync(int id, Field field)
        {
            var exsitingField = await GetFieldAsync(id, true);

            if (exsitingField == null)
            {
                throw new FieldNotExistsException();
            }

            if (field.Validations != null && field.Validations.Count > 0)
            {
                foreach (var validation in field.Validations)
                {
                    if (exsitingField.Validations.Count > 0 && exsitingField.Validations.Any(v => v.Id == validation.Id))
                    {
                        await UpdateValidationAsync(id, validation.Id, validation, false);
                    }
                    else
                    {
                        await AddValidationAsync(id, validation, false);
                    }
                }
            }

            await _connection.ExecuteScalarAsync<int>($"{_internalSchemaName}.[base_Field_Update]", new
            {
                Id = id,
                //TableName = field.TableName,
                //Name = field.Name,
                Title = field.Title,
                //Type = field.Type,
                IsHidden = field.IsHidden, IsDeleted = field.IsDeleted, IsSystem = field.IsSystem,
                // InternalName = field.InternalName,
                IsAutoGenerated = field.IsAutoGenerated, ShowInUi = field.ShowInUi, UiIndex = field.UiIndex
            }, commandType: CommandType.StoredProcedure);

            return id;
        }

        #endregion

        #region Field validation manipulations

        /// <exception cref="ValidationNotExistsException">Throws when validation not exists.</exception>
        public async Task<int> UpdateValidationAsync(int fieldId, int validationId, FieldValidation validation, bool checkExists = true)
        {
            LoadConnection();

            if (checkExists && await GetValidationAsync(fieldId, validationId, true) == null)
                throw new ValidationNotExistsException();


            await _connection.ExecuteScalarAsync<int>($"{_internalSchemaName}.[base_FieldValidation_Update]", new
            {
                Id = validationId, ValidatorId = validation.ValidatorId, Message = validation.Message, Data = validation.Data
            }, commandType: CommandType.StoredProcedure);

            return validationId;
        }

        /// <exception cref="FieldNotExistsException">Throws when field not exists.</exception>
        public async Task<int> AddValidationAsync(int fieldId, FieldValidation validation, bool checkExists = true)
        {
            LoadConnection();

            if (checkExists && await GetFieldAsync(fieldId, true) == null)
                throw new FieldNotExistsException();

            var validationId = await _connection.ExecuteScalarAsync<int>($"{_internalSchemaName}.[base_FieldValidation_Add]", new
            {
                FieldId = fieldId, ValidatorId = validation.ValidatorId, Message = validation.Message, Data = validation.Data
            }, commandType: CommandType.StoredProcedure);

            return validationId;
        }

        public async Task<FieldValidation> GetValidationAsync(int fieldId, int validationId, bool force)
        {
            if (!force) return (await GetFieldAsync(fieldId)).Validations.FirstOrDefault(v => v.Id == validationId);

            LoadConnection();

            return (await _connection.QueryAsync<FieldValidation>($"{_internalSchemaName}.[base_FieldValidation_GetById]", new
            {
                id = validationId
            }, commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<List<FieldValidation>> GetValidationsAsync(int fieldId, bool force)
        {
            if (!force) return (await GetFieldAsync(fieldId)).Validations;

            LoadConnection();

            return (await _connection.QueryAsync<FieldValidation>($"{_internalSchemaName}.[base_FieldValidation_GetByFieldId]", new
            {
                fieldId = fieldId
            }, commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task<int> DeleteValidationAsync(int id)
        {
            LoadConnection();

            try
            {
                await _connection.ExecuteAsync($"{_internalSchemaName}.[base_FieldValidation_Delete]", new
                {
                    id = id
                }, commandType: CommandType.StoredProcedure);

                return id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        private void LoadConnection()
        {
            if (_connection != null) return;
            var connectionString = _connectionString ?? ConfigurationManager.ConnectionStrings[0].ConnectionString;
            _connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
