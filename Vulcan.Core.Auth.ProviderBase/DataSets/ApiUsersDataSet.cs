using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vulcan.Core.Auth.Exceptions;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.Auth.Models.Enums;
using Vulcan.Core.Auth.Providers;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.DataSets
{
    public class ApiUsersDataSet : DataSet
    {
        public ApiUsersDataSet(DynamicDataContext context) : base(context)
        {
        }

        public async Task<List<ApiUserInternal>> SearchUsers(string searchText)
        {
            return (await Context.QueryAsync<ApiUserInternal>(
                "[auth]",
                "[auth_ApiUsers_Search]",
                new { SearchText = searchText },
                commandType: CommandType.StoredProcedure)).ToList();
        }

        public List<ApiUserInternal> GetApiUsersByGroupId(long groupid)
        {
            return (Context.Query<ApiUserInternal>(
                    "[auth]",
                    "[auth_ApiUsers_GetByGroupId]",
                    new { groupid = groupid },
                    commandType: CommandType.StoredProcedure)).ToList();
        }

        public ApiUserInternal GetUserByUsername(string username)
        {
            return (Context.Query<ApiUserInternal>(
                "[auth]",
                "[auth_ApiUsers_GetByUsername]",
                new { username = username },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
        public ApiUserInternal GetUserBySystemId(long id)
        {
            return (Context.Query<ApiUserInternal>(
                "[auth]",
                "[auth_ApiUsers_GetBySystemId]",
                new { id = id },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }


        public ApiUserInternal GetUserById(long id)
        {
            return (Context.Query<ApiUserInternal>(
                "[auth]",
                "[auth_ApiUsers_GetById]",
                new { id = id },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public async Task<ApiUser> AddApiUserAsync(ApiUser apiUser)
        {
            return await AddApiUserAsync(apiUser, ApiUserType.Internal, null, null);
        }

        public async Task<ApiUser> AddApiUserAsync(ApiUser apiUser, int externalRefId, string externalProviderName)
        {
            return await AddApiUserAsync(apiUser, ApiUserType.External, externalRefId, externalProviderName);
        }

        /// <exception cref="UsernameExistsException">Throws when added username exists on database.</exception>
        private async Task<ApiUser> AddApiUserAsync(ApiUser apiUser, ApiUserType type, int? externalRefId, string externalProviderName)
        {
            if (GetUserByUsername(apiUser.Username) != null)
            {
                throw new UsernameExistsException();
            }

            var hash = type == ApiUserType.External && apiUser.Password == null ? new HashObject() : PasswordHashProvider.CreateHash(apiUser.Password);

            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            var id =
                (await
                    Context.ExecuteScalarAsync(
                        "[auth]",
                        "[auth_ApiUsers_Add]",
                        new
                        {
                            Username = apiUser.Username,
                            PasswordHash = hash.Hash,
                            PasswordSalt = hash.Salt,
                            TokenExpireTimeMinutes = apiUser.TokenExpireTimeMinutes,
                            CreatedByUsername = identity.Name,
                            CreatedByClientId = identity.ClientId,
                            Type = type,
                            ExternalRefId = externalRefId,
                            ExternalProviderName = externalProviderName,
                            FirstName = apiUser.FirstName,
                            LastName = apiUser.LastName,
                            DisplayName = apiUser.DisplayName
                        },
                        commandType: CommandType.StoredProcedure));
            apiUser.Id = long.Parse(id.ToString());

            return apiUser;
        }

        /// <exception cref="ApiUserNotExistsException">Throws when ApiUser not exists in database.</exception>
        /// <exception cref="UsernameExistsException">Throws when updated username exists in database.</exception>
        public async Task<ApiUser> UpdateApiUserAsync(long systemId, ApiUser apiUser, ApiUserType type = ApiUserType.Internal, int? externalRefId = null)
        {
            var existingUser = GetUserBySystemId(systemId);

            if (existingUser == null) throw new ApiUserNotExistsException();

            if (existingUser.Username != apiUser.Username && GetUserByUsername(apiUser.Username) != null)
                throw new UsernameExistsException();

            var hash = type == ApiUserType.External && apiUser.Password == null ? new HashObject() : PasswordHashProvider.CreateHash(apiUser.Password);

            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);

            await
                Context.ExecuteScalarAsync(
                    "[auth]",
                    "[auth_ApiUsers_Update]",
                    new
                    {
                        Id = apiUser.Id,
                        Username = apiUser.Username,
                        PasswordHash = hash.Hash,
                        PasswordSalt = hash.Salt,
                        TokenExpireTimeMinutes = apiUser.TokenExpireTimeMinutes,
                        UpdatedByUsername = identity.Name,
                        UpdatedByClientId = identity.ClientId,
                        FirstName = apiUser.FirstName,
                        LastName = apiUser.LastName,
                        DisplayName = apiUser.DisplayName
                    },
                    commandType: CommandType.StoredProcedure);

            return apiUser;
        }

        /// <exception cref="ApiUserNotExistsException">Throws when ApiUser not exists in database.</exception>
        /// <exception cref="UsernameExistsException">Throws when updated username exists in database.</exception>
        public ApiUser UpdateApiUser(long systemId, ApiUser apiUser, ApiUserType type = ApiUserType.Internal, int? externalRefId = null)
        {
            var existingUser = GetUserBySystemId(systemId);

            if (existingUser == null) throw new ApiUserNotExistsException();

            if (existingUser.Username != apiUser.Username && GetUserByUsername(apiUser.Username) != null)
                throw new UsernameExistsException();

            var hash = type == ApiUserType.External && apiUser.Password == null ? new HashObject() : PasswordHashProvider.CreateHash(apiUser.Password);

            //var userInfo = HttpContext.Current.User.ExtractClaimPrinciple();
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);
            Context.ExecuteScalar(
                   "[auth]",
                   "[auth_ApiUsers_Update]",
                   new
                   {
                       Id = apiUser.Id,
                       Username = apiUser.Username,
                       PasswordHash = hash.Hash,
                       PasswordSalt = hash.Salt,
                       TokenExpireTimeMinutes = apiUser.TokenExpireTimeMinutes,
                       UpdatedByUsername = identity.Name,
                       UpdatedByClientId = identity.ClientId,
                       FirstName = apiUser.FirstName,
                       LastName = apiUser.LastName,
                       DisplayName = apiUser.DisplayName
                   },
                   commandType: CommandType.StoredProcedure);

            return apiUser;
        }

        public void UpdateApiUserTokenExpireTime(long userSystemId, int expireTimeMinutes)
        {
            Context.ExecuteScalar(
                   "[auth]",
                   "[auth_ApiUsers_UpdateTokenExpireTime]",
                   new
                   {
                       UserSystemId = userSystemId,
                       TokenExpireTimeMinutes = expireTimeMinutes
                   },
                   commandType: CommandType.StoredProcedure);
        }


        /// <exception cref="ApiUserNotExistsException">Throws when ApiUser not exists.</exception>
        public async Task DeleteApiUserAsync(long id)
        {
            var existingUser = (Context.Query<ApiUserInternal>(
                "[auth]",
                "[auth].[auth_ApiClientOrigins_GetById]",
                new { id = id },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            if (existingUser == null) throw new ApiUserNotExistsException();

            await Context.ExecuteScalarAsync(
                          "[auth]",
                          "[auth_ApiUsers_Delete]",
                          new
                          {
                              id = id
                          },
                          commandType: CommandType.StoredProcedure);


        }
    }
}