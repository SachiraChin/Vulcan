using Vulcan.Core.DataAccess;

namespace TestBusinessLogic
{
    public class EmployeeDataContext: DynamicDataContext
    {
        public EmployeeDataContext(string tenantId)
            : base("EmpDataContext", tenantId)
        {
        }
    }
}
