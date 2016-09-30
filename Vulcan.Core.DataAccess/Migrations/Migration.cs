using System;
using System.Collections.Generic;
using Vulcan.Core.DataAccess.Migrations.MigrationProviders;

namespace Vulcan.Core.DataAccess.Migrations
{
    public class Migration
    {
        public List<IMigrationProvider> MigrationEntries { get; set; }
        public Guid MigrationId { get; set; }
        public List<Guid> Prerequisites { get; set; }
    }
}
