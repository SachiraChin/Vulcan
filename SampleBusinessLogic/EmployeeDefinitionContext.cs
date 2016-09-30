using Vulcan.Core.DataAccess;

namespace TestBusinessLogic
{
    public class EmployeeDefinitionContext : DataDefinitionContext
    {

        public EmployeeDefinitionContext(string tenantId)
            : base("EmpDataContext", tenantId)
        {
        }
    }
}
