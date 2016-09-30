using System;

namespace Vulcan.Core.DataAccess.Constraints.Providers
{
    public class RequiredConstraintProvider : IConstraintProvider
    {
        public int Create(int fieldId, string constraintName)
        {
            throw new NotImplementedException();
        }

        public void Remove(int fieldId, string constraintName)
        {
            throw new NotImplementedException();
        }
    }
}
