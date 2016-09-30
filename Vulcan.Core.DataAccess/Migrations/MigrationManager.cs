using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vulcan.Core.DataAccess.Migrations
{
    public class MigrationManager : IDisposable
    {
        public enum InputType
        {
            Xml,
            Json
        }
        
        public DataDefinitionContext DataDefinitionContext { get; }
        
        public MigrationManager(DataDefinitionContext dataDefinitionContext)
        {
            DataDefinitionContext = dataDefinitionContext;
        }

        public async Task Migrate(string content, InputType type, bool baseMigration = false)
        {
            Migration migration;
            switch (type)
            {
                case InputType.Xml:
                    using (TextReader reader = new StringReader(content))
                    {
                        migration = (Migration)new XmlSerializer(typeof(Migration)).Deserialize(reader);
                    }
                    break;
                case InputType.Json:
                    migration = JsonConvert.DeserializeObject<Migration>(content);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            await Migrate(migration, baseMigration);
        }

        public async Task Migrate(Stream stream, InputType type, bool baseMigration = false)
        {
            Migration migration;
            switch (type)
            {
                case InputType.Xml:
                    var xmlSerializer = new XmlSerializer(typeof(Migration));
                    migration = (Migration)xmlSerializer.Deserialize(stream);
                    break;
                case InputType.Json:
                    var jsonSerializer = new JsonSerializer();
                    migration = jsonSerializer.Deserialize<Migration>(new JsonTextReader(new StreamReader(stream)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }
            await Migrate(migration, baseMigration);
        }

        public async Task Migrate(JObject jsonObject, bool baseMigration = false)
        {
            await Migrate(jsonObject.ToObject<Migration>(), baseMigration);
        }

        public async Task Migrate(XDocument xmlDocument, bool baseMigration = false)
        {
            if (xmlDocument.Root != null)
            {
                var xmlSerializer = new XmlSerializer(typeof(Migration));
                using (var reader = xmlDocument.Root.CreateReader())
                {
                    await Migrate((Migration)xmlSerializer.Deserialize(reader), baseMigration);
                }
            }
        }

        public async Task Migrate(Migration migration, bool baseMigration = false)
        {
            if (migration != null)
            {
                if (!baseMigration && migration.Prerequisites != null && migration.Prerequisites.Count > 0)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("Value", typeof(Guid));
                    migration.Prerequisites.ForEach(p => dt.Rows.Add(p));

                    var count = await DataDefinitionContext.ExecuteScalarAsync<int>("core", "[base_Migration_GetExistingCount]",
                        new
                        {
                            ids = dt
                        },
                        commandType: CommandType.StoredProcedure);

                    if (count != migration.Prerequisites.Count)
                    {
                        throw new Exception("Migration prerequisites not satisfied");
                    }
                }
                if (!baseMigration)
                {
                    await DataDefinitionContext.ExecuteScalarAsync<int>("core", "[base_Migration_Add]",
                        new { id = migration.MigrationId },
                        commandType: CommandType.StoredProcedure);
                }
                var migrationIndex = 0;
                foreach (var migrationEntry in migration.MigrationEntries)
                {
                    await migrationEntry.Process(DataDefinitionContext);

                    if (baseMigration) continue;

                    var json = JsonConvert.SerializeObject(migrationEntry);
                    await
                        DataDefinitionContext.ExecuteScalarAsync<int>(
                            "core",
                            "[base_MigrationEntry_Add]",
                            new
                            {
                                MigrationId = migration.MigrationId,
                                EntryJson = json,
                                ExecutionOrderIndex = migrationIndex
                            },
                            commandType: CommandType.StoredProcedure);

                    migrationIndex++;
                }
            }
            else
            {
                throw new Exception("Migration cannot be null");
            }
        }

        public void Dispose()
        {
            DataDefinitionContext.Dispose();
        }
    }
}
