using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using Vulcan.Core.Utilities;

namespace Vulcan.Core.DataAccess.Migrations.MigrationProviders.Providers
{
    public static class DacFxUtils
    {
        public static void DeployDacpac(string dacpacPath, string connectionString, string databaseName, Dictionary<string, string> variables = null, Action<string> logWriter = null)
        {
            //Class responsible for the deployment. (Connection string supplied by console input for now)
            var dbServices = new DacServices(connectionString);

            //Wire up events for Deploy messages and for task progress (For less verbose output, don't subscribe to Message Event (handy for debugging perhaps?)
            if (logWriter != null)
            {
                dbServices.Message += new EventHandler<DacMessageEventArgs>((sender, args) =>
                {
                    logWriter.Invoke(DateTime.Now.ToString("s") + " " + args.Message.ToString());
                });
                dbServices.ProgressChanged += new EventHandler<DacProgressEventArgs>((sender, args) =>
                {
                    logWriter.Invoke(DateTime.Now.ToString("s") + $" Operation {args.OperationId}: {args.Status.ToString()}: {args.Message}");
                });
            }

            //This Snapshot should be created by our build process using MSDeploy
            var dbPackage = DacPackage.Load(dacpacPath);




            var dbDeployOptions = new DacDeployOptions();
            //Cut out a lot of options here for configuring deployment, but are all part of DacDeployOptions
            dbDeployOptions.SqlCommandVariableValues.Add("debug", "false");
            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    dbDeployOptions.SqlCommandVariableValues.Add(variable.Key, variable.Value);
                }
            }

            dbServices.Deploy(dbPackage, databaseName, true, dbDeployOptions);
        }

        public static void SubstituteSchemaInModel(ref TSqlModel model, string oldSchema, string newSchema, Action<string> logWriter)
        {
            var sourceModelObjects = model.GetObjects(DacQueryScopes.UserDefined).ToList();

            var i = 0;
            foreach (var tsqlObject in sourceModelObjects)
            {
                logWriter?.Invoke(DateTime.Now.ToString("s") + " Processing object " + (i + 1) + " of " + sourceModelObjects.Count + ": " + tsqlObject.ObjectType.Name + " " + tsqlObject.Name);

                i++;

                var sourceInfo = tsqlObject.GetSourceInformation();
                string oldObjectScript;
                tsqlObject.TryGetScript(out oldObjectScript);
                var newObjectScript = string.Empty;
                IList<ParseError> errors;
                try
                {
                    var parser = new TSql120Parser(false);
                    var fragment = (TSqlScript)parser.Parse(new StringReader(oldObjectScript), out errors);
                    if (fragment.Batches.Count > 0 && fragment.Batches[0].Statements.Count > 0)
                    {
                        var stmt = fragment.Batches[0].Statements[0];
                        ReflectionUtils.SubstituteSchemaName(stmt, oldSchema, newSchema, "Microsoft.SqlServer.TransactSql.ScriptDom");
                        var sg = new Sql120ScriptGenerator();
                        sg.GenerateScript(fragment, out newObjectScript);
                    }
                    else
                    {
                        logWriter?.Invoke(DateTime.Now.ToString("s") + " Object " + (i + 1) + " of " + sourceModelObjects.Count + ": Statements not available");

                    }
                }
                catch (Exception ex)
                {
                    logWriter?.Invoke(DateTime.Now.ToString("s") + " Error Processing object " + (i + 1) + " of " + sourceModelObjects.Count + ": " + ex.Message);
                    logWriter?.Invoke(ex.StackTrace);
                }
                //Console.WriteLine(newObjectScript);
                //Console.WriteLine();
                //Console.WriteLine();
                //Console.WriteLine();
                if (oldObjectScript == newObjectScript || newObjectScript == string.Empty) continue;

                model.DeleteObjects(sourceInfo.SourceName);
                model.AddObjects(newObjectScript);
            }
        }

        public static string[] SubstituteSchemaInModelFromScripts(string[] scripts, string oldSchema, string newSchema, bool applyFormatting = true, Action<string> logWriter = null)
        {
            var model = new TSqlModel(SqlServerVersion.Sql120, new TSqlModelOptions { });
            foreach (var script in scripts)
            {
                model.AddObjects(script);
            }
            SubstituteSchemaInModel(ref model, oldSchema, newSchema, logWriter);
            var resultingList = new List<string>();
            foreach (var tso in model.GetObjects(DacQueryScopes.UserDefined))
            {
                string currentScript;
                tso.TryGetScript(out currentScript);
                if (applyFormatting)
                    currentScript = RemoveExtraWhiteSpace(currentScript.Replace(System.Environment.NewLine, " ")).Trim().ToUpper();
                if (currentScript.Length > 0)
                    resultingList.Add(currentScript);
            }
            return resultingList.ToArray();
        }

        public static bool SubstituteSchemaInDacpac(string fileName, Dictionary<string, string> schemaList, string saveFileName, bool overrideFile = false, Action<string> logWriter = null)
        {
            if (!overrideFile && File.Exists(saveFileName))
            {
                logWriter?.Invoke(DateTime.Now.ToString("s") + " Save file already exixts: " + saveFileName);
                return false;
            }

            // Load model from file
            logWriter?.Invoke(DateTime.Now.ToString("s") + " Loading model from file " + fileName);

            var modelFromDacpac = TSqlModel.LoadFromDacpac(fileName,
                new ModelLoadOptions(DacSchemaModelStorageType.Memory, loadAsScriptBackedModel: true));

            //Console.WriteLine(DateTime.Now.ToString("s") + " Loaded");
            //TSqlModel modelFromDacpac = new TSqlModel(fileName);
            // For each pair OldSchema/NewSchema, update the model

            //Console.WriteLine(DateTime.Now.ToString("s") + " Saving updated model to file " + fileName);
            foreach (var schema in schemaList)
            {
                SubstituteSchemaInModel(ref modelFromDacpac, schema.Key.Trim(), schema.Value.Trim(), logWriter);
            }
            //modelFromDacpac = SubstituteSchemaInModel(modelFromDacpac, oldSchemaList[i].Trim(), newSchemaList[i].Trim());
            // Save updated model to the file
            logWriter?.Invoke(DateTime.Now.ToString("s") + " Save updated model to the file");

            File.Copy(fileName, saveFileName, overrideFile);
            using (var dacPackage = DacPackage.Load(saveFileName,
                DacSchemaModelStorageType.Memory,
                FileAccess.ReadWrite))
            {
                //DacPackageExtensions.BuildPackage(saveFileName, modelFromDacpac, null);
                DacPackageExtensions.UpdateModel(dacPackage, modelFromDacpac, null);
            }
            logWriter?.Invoke(DateTime.Now.ToString("s") + " Saved");


            return true;
        }
        public static string SubstituteSchemaInQuery(string query, string oldSchema, string newSchema, bool applyFormatting = true)
        {
            var parser = new TSql120Parser(false);
            IList<ParseError> errors;
            var fragment = parser.Parse(new StringReader(query), out errors);
            ReflectionUtils.SubstituteSchemaName(fragment, oldSchema, newSchema, "Microsoft.SqlServer.TransactSql.ScriptDom");
            string result;
            var sg = new Sql120ScriptGenerator();
            sg.GenerateScript(fragment, out result);
            if (applyFormatting)
                result = RemoveExtraWhiteSpace(result.Replace(System.Environment.NewLine, " ")).Trim().ToUpper();
            return result;
        }
        private static string RemoveExtraWhiteSpace(string src)
        {
            while (src.Contains("  "))  //double space
                src = src.Replace("  ", " "); //replace double space by single space.
            while (src.Contains(" (")) //space before opening brace
                src = src.Replace(" (", "(");
            while (src.Contains(" )")) //space before closing brace
                src = src.Replace(" )", ")");
            return src;
        }
    }
}
