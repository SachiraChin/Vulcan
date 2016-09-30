using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Vulcan.Core.DataAccess.Entities
{
    public class SerializableDynamicMetaObject : DynamicMetaObject
    {
        readonly Type _objType;

        public SerializableDynamicMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : base(expression, restrictions, value)
        {
            _objType = value.GetType();
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var self = this.Expression;
            var dynObj = (SerializableDynamicObject)this.Value;
            var keyExpr = Expression.Constant(binder.Name);
            var getMethod = _objType.GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var target = Expression.Call(Expression.Convert(self, _objType), getMethod, keyExpr);
            return new DynamicMetaObject(target, BindingRestrictions.GetTypeRestriction(self, _objType));
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var self = this.Expression;
            var keyExpr = Expression.Constant(binder.Name);
            var valueExpr = Expression.Convert(value.Expression, typeof(object));
            var setMethod = _objType.GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var target = Expression.Call(Expression.Convert(self, _objType), setMethod, keyExpr, valueExpr);
            return new DynamicMetaObject(target, BindingRestrictions.GetTypeRestriction(self, _objType));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var dynObj = (SerializableDynamicObject)this.Value;
            return dynObj.GetDynamicMemberNames();
        }
    }
}