using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class InterTimeZoneUTCDataSet:DataSet
    {
        public InterTimeZoneUTCDataSet(DynamicDataContext context) : base(context)
        {
        }
        public async Task<List<InternationalTimezoneUTC>> GetTimezoneUTC()
        {
            return (await Context.QueryAsync<InternationalTimezoneUTC>(
               "[auth]",
               "[auth_TimeZoneUTCs_GetAll]",
               commandType: CommandType.StoredProcedure)).ToList();
        }
    }
}
