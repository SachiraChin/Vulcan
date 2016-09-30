using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class InterTimeZonesDataSet:DataSet
    {
        public InterTimeZonesDataSet(DynamicDataContext context):base (context)
        {
        }
        public async Task<List<InternationalTimeZone>> GetTimezone()
        {
            return (await Context.QueryAsync<InternationalTimeZone>(
               "[auth]",
               "[auth_TimeZones_GetAll]",
               commandType: CommandType.StoredProcedure)).ToList();
        } 
    }
}
