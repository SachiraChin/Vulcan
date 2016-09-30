using System.Runtime.Serialization;
using System.Web.Http.ModelBinding;
using Vulcan.Core.DataAccess.Validations;

namespace Vulcan.Core.DataAccess.Entities
{
    public class DynamicEntity : SerializableDynamicObject, IValidatable
    {
        [DataMember]
        public virtual int Id { get; set; }
        public ModelStateDictionary ModelState { get; set; }
        public static T Parse<T>(dynamic entity)
            where T : DynamicEntity, new()
        {
            var obj = (dynamic)new T();
            foreach (var value in entity)
            {
                obj.SetValue(value.Key, value.Value);
            }

            return obj;
        }
    }
}
