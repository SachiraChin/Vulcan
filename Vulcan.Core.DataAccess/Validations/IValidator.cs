using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vulcan.Core.DataAccess.Constraints;

namespace Vulcan.Core.DataAccess.Validations
{
    public interface IValidator
    {
        [JsonProperty("Id")]
        int ValidationId { get; set; }
        [JsonProperty("ValidatorId")]
        Guid Id { get; }
        [JsonIgnore]
        string FieldName { get; set; }
        string Message { get; set; }
        bool Validate(object value, string data = null);
        string EncodeData();
        void DecodeData(string data);
        ValidatorType Type { get; }
        IEnumerable<IConstraintProvider> GetConstraints();
    }
}
