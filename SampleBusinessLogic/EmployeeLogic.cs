using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vulcan.Core.DataAccess;

namespace TestBusinessLogic
{
    public class EmployeeLogic : LogicBase<Employee>
    {
        public EmployeeLogic(string tenantId) : base(EntityNameProvider.TableNames.Schema, EntityNameProvider.TableNames.Employee, new EmployeeDataContext(tenantId), new EmployeeDefinitionContext(tenantId))
        {
        }
    }
}
