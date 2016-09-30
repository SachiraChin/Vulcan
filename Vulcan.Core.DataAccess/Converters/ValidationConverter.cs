using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vulcan.Core.DataAccess.Validations;

namespace Vulcan.Core.DataAccess.Converters
{
    public class ValidationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IValidator);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            string id = null;
            if (jObject["validatorId"] != null)
            {
                id = jObject["validatorId"].Value<string>();
            }
            else if (jObject["ValidatorId"] != null)
            {
                id = jObject["ValidatorId"].Value<string>();
            }
            if (id == null) return null;

            var type = ValidatorFactory.GetType(id);
            var obj = serializer.Deserialize(jObject.CreateReader(), type);
            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
