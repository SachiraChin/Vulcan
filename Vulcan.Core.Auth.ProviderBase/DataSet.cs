using Vulcan.Core.DataAccess;

namespace Vulcan.Core.Auth
{
    public abstract class DataSet
    {
        protected readonly DynamicDataContext Context;

        protected DataSet(DynamicDataContext context)
        {
            Context = context;
        }
    }
}
