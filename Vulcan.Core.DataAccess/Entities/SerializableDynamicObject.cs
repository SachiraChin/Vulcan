using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Vulcan.Core.DataAccess.Entities
{
    [DataContract]
    public class SerializableDynamicObject : IDynamicMetaObjectProvider, IEntity
    {
        //[DataMember]
        internal Dictionary<string, object> DynamicProperties = new Dictionary<string, object>();

        #region IDynamicMetaObjectProvider implementation
        public DynamicMetaObject GetMetaObject(Expression expression)
        {
            return new SerializableDynamicMetaObject(expression,
                BindingRestrictions.GetInstanceRestriction(expression, this), this);
        }
        #endregion

        #region Helper methods for dynamic meta object support
        internal object SetValue(string name, object value)
        {
            DynamicProperties.Add(name, value);
            return value;
        }

        internal object GetValue(string name)
        {
            object value;
            DynamicProperties.TryGetValue(name, out value);
            return value;
        }

        internal IEnumerable<string> GetDynamicMemberNames()
        {
            return DynamicProperties.Keys;
        }
        #endregion

        public Dictionary<string, object> EntityData
        {
            get { return DynamicProperties; }
            set { DynamicProperties = value; }
        }
    }
}