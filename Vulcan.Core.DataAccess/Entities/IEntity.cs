using System.Collections.Generic;

namespace Vulcan.Core.DataAccess.Entities
{
    public interface IEntity
    {
        Dictionary<string, object> EntityData { get; set; }
    }
}
