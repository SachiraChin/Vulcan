using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.Auth.DataSets
{
    public class OrganizationsDataSet:DataSet
    {
        public OrganizationsDataSet(DynamicDataContext context) : base(context)
        {
        }
        public async Task<Organization> GetOrganizationAsync()
        {
            return (await Context.QueryAsync<Organization>(
                "[auth]",
                "[auth_Organizations_Get]",
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
        public async Task<Organization> AddOrganizationAsync(Organization organization)
        {
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);
            await Context.ExecuteScalarAsync(
                "[auth]",
                "[auth_Organizations_Add]",
                new
                {
                    Name = organization.Name,
                    Address = organization.Address,
                    Email = organization.Email,
                    Timezone = organization.Timezone,
                    CreatedByUsername = identity.Name,
                    CreatedByClientId = identity.ClientId
                },
                commandType: CommandType.StoredProcedure);
            return organization; 
        }
     public async Task<Organization> UpdateOrganizationAsync(Organization organization)
        {
            //var exisitingOrganization = GetOrganizationById(id);
            //exception
            var identity = new TenantUserIdentity(HttpContext.Current.User.Identity);
            await Context.ExecuteScalarAsync(
                "[auth]",
                "[auth_Organizations_Update]",
                new
                {
                    Id = organization.Id,
                    Name = organization.Name,
                    Address = organization.Address,
                    Email = organization.Email,
                    Timezone = organization.Timezone,
                    UpdatedByUsername = identity.Name,
                    UpdatedByClientId = identity.ClientId
                },
                commandType: CommandType.StoredProcedure);

            return organization;
        }
    }
}
