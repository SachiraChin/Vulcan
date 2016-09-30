using System.Collections.Generic;

namespace Vulcan.Core.Auth.Models
{
    public class InternationalTimeZone
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Abbr { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
        public List<InternationalTimezoneUTC> UTCs { get; set; }
    }
}
