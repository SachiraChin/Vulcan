using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.Core.Auth.AzureAuthProvider.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.AzureAuthProvider.DataSets
{
    public class AzureDataSet : DataSet
    {
        public AzureDataSet(DynamicDataContext context) : base(context)
        {
        }

        public async Task<int> AddUserAsync(AzureJwtPayload payload)
        {
            var id =
                (await
                    Context.ExecuteScalarAsync(
                        "[aad]",
                        "[az_User_Add]",
                        new
                        {
                            oid = payload.oid,
                            tid = payload.tid,
                            upn = payload.upn,
                            unique_name = payload.unique_name,
                            family_name = payload.family_name,
                            given_name = payload.given_name
                        },
                        commandType: CommandType.StoredProcedure));

            return int.Parse(id.ToString());
        }
        public async Task UpdateUserAsync(int userId, AzureJwtPayload payload)
        {
            await
                Context.ExecuteScalarAsync(
                    "[aad]",
                    "[az_User_Update]",
                    new
                    {
                        UserId = userId,
                        oid = payload.oid,
                        tid = payload.tid,
                        upn = payload.upn,
                        unique_name = payload.unique_name,
                        family_name = payload.family_name,
                        given_name = payload.given_name
                    },
                    commandType: CommandType.StoredProcedure);

        }


        public async Task AddAccessCodeAsync(long apiUserId, string accessCode, int expiresOn)
        {
            var expireOn = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiresOn);
            await
                Context.ExecuteScalarAsync(
                    "[aad]",
                    "[az_AccessCodes_Add]",
                    new
                    {
                        ApiUserId = apiUserId,
                        AccessCode = accessCode,
                        ExpiresOn = expireOn
                    },
                    commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAccessCodeAsync(int apiUserId, string accessCode, int expiresOn)
        {
            var expireOn = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiresOn);
            await
                Context.ExecuteScalarAsync(
                    "[aad]",
                    "[az_AccessCodes_Update]",
                    new
                    {
                        ApiUserId = apiUserId,
                        AccessCode = accessCode,
                        ExpiresOn = expireOn
                    },
                    commandType: CommandType.StoredProcedure);
        }

        public void UpdateAccessCode(int apiUserId, string accessCode, int expiresOn)
        {
            var expireOn = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiresOn);
            Context.ExecuteScalar(
                    "[aad]",
                    "[az_AccessCodes_Update]",
                    new
                    {
                        ApiUserId = apiUserId,
                        AccessCode = accessCode,
                        ExpiresOn = expireOn
                    },
                    commandType: CommandType.StoredProcedure);
        }

        public TempAccessCode GetAccessCode(string code)
        {
            return (Context.Query<TempAccessCode>(
                "[aad]",
                "[az_AccessCodes_GetByCode]",
                new { code = code },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public Models.AzureRefreshToken GetRefreshToken(int userId)
        {
            return (Context.Query<Models.AzureRefreshToken>(
                "[aad]",
                "[az_RefreshTokens_GetByUserId]",
                new { userId = userId },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();

            //return token != null ? new {refresh_token = token.RefreshToken, expires_on = token.ExpiresOn, expires_in = token.ExpireIn } : null;
        }


        public void DeactivateAccessCode(long userId)
        {
            Context.ExecuteScalar(
                "[aad]",
                "[az_AccessCodes_Deactivate]",
                new { userId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public void UpdateRefreshToken(int userId, AzureToken token, AzureJwtPayload payload)
        {
            var expireOn = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(token.expires_on);
            Context.ExecuteScalar(
                    "[aad]",
                    "[az_RefreshTokens_Update]",
                    new
                    {
                        UserId = userId,
                        ExpireIn = token.expires_in,
                        ExpiresOn = expireOn,
                        RefreshToken = token.refresh_token,
                        AccessToken = token.access_token,
                        ExternalTenantId = payload.tid
                    },
                    commandType: CommandType.StoredProcedure);
        }
        public async Task AddRefreshToken(int userId, AzureToken token, AzureJwtPayload payload)
        {
            var expireOn = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(token.expires_on);
            await Context.ExecuteScalarAsync(
                    "[aad]",
                    "[az_RefreshToken_Add]",
                    new
                    {
                        UserId = userId,
                        ExpireIn = token.expires_in,
                        ExpiresOn = expireOn,
                        RefreshToken = token.refresh_token,
                        AccessToken = token.access_token,
                        ExternalTenantId = payload.tid
                    },
                    commandType: CommandType.StoredProcedure);
        }
    }
}
