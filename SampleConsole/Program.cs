using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestBusinessLogic;
using System.Web.ModelBinding;
using Microsoft.ServiceBus.Messaging;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Vulcan.Core.DataAccess.Migrations;
using Vulcan.Core.DataAccess.Migrations.MigrationProviders;
using Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers;
using Vulcan.Core.DataAccess.Models;
using Vulcan.Core.DataAccess.Validations.Validators;

namespace TestConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //PopulateTable().Wait();
            InvokeJob();
            Console.Read();
        }

        public static void InvokeJob()
        {
            var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString, "Vulcancoretenantdatabasegeneratequeue");

            var message = new BrokeredMessage("Test3");
            
            client.Send(new BrokeredMessage(Guid.NewGuid().ToString("N")));
            client.Send(new BrokeredMessage(Guid.NewGuid().ToString("N")));
        }

        public static async Task PopulateTable()
        {
            var tid = $"{new Guid("{81EA92BE-FF52-43CF-A13B-0CB31139D919}").ToString("N")}";
            var filename = $"{tid}.dacpac";
            DacFxUtils.SubstituteSchemaInDacpac("Vulcan.Core.Auth.Database.dacpac", new Dictionary<string, string>()
            {
                //["core"] = $"{tid}_core",
                ["auth"] = $"{tid}_auth"
            }, filename, true);
            DacFxUtils.DeployDacpac(filename, ConfigurationManager.ConnectionStrings["EmpDataContext"].ConnectionString, "trunk",
                new Dictionary<string, string>()
                {
                    ["NewSchema"] = $"{tid}_auth"
                });

            using (var empLogic = new EmployeeLogic(tid))
            {
                var fields = empLogic.GetFields(true);
                if (fields.Count() != 5)
                {
                    var sch = new SchemaMigrationProvider() { SchemaName = EntityNameProvider.TableNames.Schema };
                    var tab = new TableMigrationProvider() { TableName = EntityNameProvider.TableNames.Employee };
                    var f1 = new FieldMigrationProvider()
                    {
                        Field = new Field()
                        {
                            TableName = EntityNameProvider.TableNames.Employee,
                            Name = "Age",
                            Title = "Age",
                            Type = FieldType.Number,
                            Validations = new List<FieldValidation>()
                                    {
                                        new FieldValidation(new RegExValidator
                                            {
                                                RegexOptions = RegexOptions.IgnoreCase,
                                                RegularExpression = @"^\d+$"
                                            }) { Message = "Not a number" } ,
                                        new FieldValidation(new RangeValidator()
                                            {
                                                MinValue = 0,
                                                MaxValue = 120,
                                                Mode = RangeValidator.RangeValidatorType.MinMaxIncluded
                                            }) { Message = "Invalid age" }
                                    }
                        },
                        Type = ExecutionType.Insert
                    };
                    var f2 = new FieldMigrationProvider()
                    {
                        Field = new Field()
                        {
                            TableName = EntityNameProvider.TableNames.Employee,
                            Name = "FirstName",
                            Title = "First Name",
                            Type = FieldType.Text,
                            Validations = new List<FieldValidation>()
                                    {
                                        new FieldValidation(new RequiredValidator()) { Message = "First name is required" }
                                    }
                        },
                        Type = ExecutionType.Insert
                    };
                    var f3 = new FieldMigrationProvider()
                    {
                        Field = new Field()
                        {
                            TableName = EntityNameProvider.TableNames.Employee,
                            Name = "LastName",
                            Title = "Last Name",
                            Type = FieldType.Text,
                            Validations = new List<FieldValidation>()
                        },
                        Type = ExecutionType.Insert
                    };

                    var f4 = new FieldMigrationProvider()
                    {
                        Field = new Field()
                        {
                            TableName = EntityNameProvider.TableNames.Employee,
                            Name = "Email",
                            Title = "Email",
                            Type = FieldType.Text,
                            Validations = new List<FieldValidation>()
                                    {
                                        new FieldValidation(new EmailValidator()) { Message = "Invalid email address" },
                                        new FieldValidation(new RequiredValidator()) { Message = "Email is required" }
                                    }
                        },
                        Type = ExecutionType.Insert
                    };


                    var migration = new Migration
                    {
                        MigrationId = Guid.NewGuid(),
                        MigrationEntries = new List<IMigrationProvider>()
                                {
                                    sch, tab, f1, f2, f3, f4
                                }
                    };

                    var migrationMan = new MigrationManager(empLogic.DataDefinitionContext);
                    await migrationMan.Migrate(migration, false);

                    empLogic.GetFields(true);
                }

                dynamic emp1 = new Employee();
                emp1.FirstName = "Sachira";
                emp1.LastName = "Jayasanka";
                emp1.Age = 26;
                emp1.Email = "sachira@live.com";

                if (empLogic.Validate(emp1))
                {
                    await empLogic.AddAsync(emp1);
                }
                else
                {
                    PrintError(emp1);
                }
            }

            Console.WriteLine("done");
        }

        private static void PrintError(Employee stateDic)
        {
            foreach (var field in stateDic.ModelState)
            {
                var fieldName = field.Key;
                Console.WriteLine($"Field {fieldName} has error(s):");
                var state = field.Value;
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"\t{error.ErrorMessage}");
                }

                Console.WriteLine();
            }
        }
    }
}

