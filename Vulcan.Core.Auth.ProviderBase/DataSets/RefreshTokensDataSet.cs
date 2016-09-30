using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class RefreshTokensDataSet : DataSet
    {
        public RefreshTokensDataSet(DynamicDataContext context) : base(context)
        {
        }

        public async Task AddTokenAsync(RefreshToken token)
        {
            await Context.ExecuteScalarAsync(
                "[auth]",
                "[auth_RefreshTokens_Add]",
                new
                {
                    TokenHash = token.TokenHash,
                    Subject = token.Subject,
                    IssuedDate = token.IssuedDate,
                    ExpiryDate = token.ExpiryDate,
                    Ticket = token.Ticket
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteByHashAsync(string hash)
        {
            await Context.ExecuteScalarAsync(
                "[auth]",
                "[auth_RefreshTokens_DeleteByTokenHash]",
                new { tokenHash = hash },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RefreshToken> GetByHashAsync(string hash)
        {
            return (await Context.QueryAsync<RefreshToken>(
                "[auth]",
                "[auth_RefreshTokens_GetByTokenHash]",
                new { tokenHash = hash },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
    }
}