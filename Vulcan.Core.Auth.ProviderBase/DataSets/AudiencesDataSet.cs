using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vulcan.Core.Auth.Models;
using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth.DataSets
{
    public class AudiencesDataSet : DataSet
    {
        public AudiencesDataSet(DynamicDataContext context) : base(context)
        {
        }

        public Audience GetAudienceByAudinceId(string audinceId)
        {
            return (Context.Query<Audience>(
                "[auth]",
                "[auth_Audiences_GetByAudienceId]",
                new { audinceId = audinceId },
                commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }

        public List<Audience> GetAudiences()
        {
            return Context.Query<Audience>(
                "[auth]",
                "[auth_Audiences_GetAll]",
                commandType: CommandType.StoredProcedure).ToList();
        }
    }
}