using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vulcan.Core.Auth.Exceptions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Providers;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.DataSets
{
    public class ApiClientsDataSet : DataSet
    {
        public ApiClientsDataSet(DynamicDataContext context) : base(context)
        {
        }

        public ApiClientInternal GetApiClientByClientId(string clientId, bool includeOrigins = false)
        {
            var client = (Context.Query<ApiClientInternal>(
                "[auth]",
                "[auth_ApiClients_GetByClientId]",
                new { clientId = clientId },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (!includeOrigins || client == null) return client;

            var origins = (Context.Query<ApiClientOrigin>(
                "[auth]",
                "[auth_ApiClientOrigins_GetByClientId]",
                new { clientId = clientId },
                commandType: CommandType.StoredProcedure)).ToList();
            client.ApiClientOrigins = origins;

            return client;
        }

        public async Task<ApiClient> AddApiClientAsync(ApiClient apiClient)
        {
            var clientId = Guid.NewGuid().ToString("N");
            while (GetApiClientByClientId(clientId, false) != null)
            {
                clientId = Guid.NewGuid().ToString("N");
            }

            apiClient.ClientId = clientId;
            apiClient.ClientSecret = ApiKeyProvider.GenerateKey();
            var hash = PasswordHashProvider.CreateHash(apiClient.ClientSecret);
            var clientSecretHash = hash.Hash;
            var clientSecretSalt = hash.Salt;
            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            var id =
                (await
                    Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_ApiClients_Add]",
                        new
                        {
                            ClientId = apiClient.ClientId,
                            ClientSecretHash = clientSecretHash,
                            ClientSecretSalt = clientSecretSalt,
                            Type = apiClient.Type,
                            TokenExpireTimeMinutes = apiClient.TokenExpireTimeMinutes,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure));
            apiClient.Id = long.Parse(id.ToString());

            return apiClient;
        }

        /// <exception cref="ApiClientNotExistsException">Throws when ApiClient not exists.</exception>
        public async Task<ApiClient> UpdateApiClientAsync(long id, ApiClient apiClient, bool resetClientId, bool resetSecret)
        {

            var existingClient = (Context.Query<ApiClientInternal>(
                "[auth]",
                "[auth_ApiClients_GetBySystemId]",
                new { id = id },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (existingClient == null) throw new ApiClientNotExistsException();

            var clientId = apiClient.ClientId;
            string clientSecretHash = null;
            string clientSecretSalt = null;

            if (resetClientId)
            {
                clientId = Guid.NewGuid().ToString("N");
                while (GetApiClientByClientId(clientId, false) != null)
                {
                    clientId = Guid.NewGuid().ToString("N");
                }

                apiClient.ClientId = clientId;
            }

            if (resetSecret)
            {
                apiClient.ClientSecret = ApiKeyProvider.GenerateKey();
                var hash = PasswordHashProvider.CreateHash(apiClient.ClientSecret);
                clientSecretHash = hash.Hash;
                clientSecretSalt = hash.Salt;
            }

            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);
            await Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_ApiClients_Update]",
                        new
                        {
                            Id = id,
                            ClientId = clientId,
                            ResetSecret = resetSecret,
                            ClientSecretHash = clientSecretHash,
                            ClientSecretSalt = clientSecretSalt,
                            Type = apiClient.Type,
                            TokenExpireTimeMinutes = apiClient.TokenExpireTimeMinutes,
                            UpdatedByUsername = identity.Name,
                            UpdatedByClientId = identity.ClientId
                        },
                        commandType: CommandType.StoredProcedure);

            return apiClient;

        }

        public async Task DeleteApiClientAsync(long id)
        {
            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_ApiClients_Delete]",
                          new
                          {
                              id = id
                          },
                          commandType: CommandType.StoredProcedure);


        }

        public async Task AddOriginAsync(long id, string origin)
        {
            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_ApiClientOrigins_Add]",
                          new
                          {
                              ClientId = id,
                              Origin = origin
                          },
                          commandType: CommandType.StoredProcedure);
        }

        public async Task<List<ApiClientOrigin>> GetOriginsAsync(string clientId)
        {
            return (await Context.QueryAsync<ApiClientOrigin>(
                "[auth]",
                "[auth_ApiClientOrigins_GetByClientId]",
                new { clientId = clientId },
                commandType: CommandType.StoredProcedure)).ToList();
        }

        public async Task DeleteApiClientOriginAsync(int originId)
        {
            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_ApiClientOrigins_Delete]",
                          new
                          {
                              originId = originId
                          },
                          commandType: CommandType.StoredProcedure);


        }

    }
}