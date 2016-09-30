namespace Vulcan.Core.DataAccess.Constraints
{
    public interface IConstraintProvider
    {
        int Create(int fieldId, string constraintName);
        void Remove(int fieldId, string constraintName);
    }
}
