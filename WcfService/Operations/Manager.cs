using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Oyosoft.AgenceImmobiliere.Core.DataAccess;
using Oyosoft.AgenceImmobiliere.Core.Service;
using Oyosoft.AgenceImmobiliere.Core.Tools;

namespace Oyosoft.AgenceImmobiliere.WcfService.Operations
{
    internal static class Manager
    {
        internal static TResult ExecuteOperationSynchronously<TResult, TParam1, TParam2>(Func<Connection, TParam1, TParam2[], Task<TResult>> operation, TParam1 parameter, params TParam2[] parameters) where TResult : OperationResult, new()
        {
            return ExecuteOperation(operation, parameter, parameters).ExecuteSynchronously();
        }

        internal async static Task<TResult> ExecuteOperation<TResult, TParam1, TParam2>(Func<Connection, TParam1, TParam2[], Task<TResult>> operation, TParam1 parameter, params TParam2[] parameters) where TResult : OperationResult, new()
        {
            TResult result = null;
            Connection conn = null;

            // Récupération du chemin à la base de données
            string databasePath = "";
            try
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.DATABASE_DIRECTORY_PATH))
                {
                    databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Properties.Settings.Default.DATABASE_FILE_NAME);
                }
                else
                {
                    databasePath = Path.Combine(Properties.Settings.Default.DATABASE_DIRECTORY_PATH, Properties.Settings.Default.DATABASE_FILE_NAME);
                }
            } catch { }
            

            try
            {
                // Initialisation de la connexion à la base de données
                conn = await Connection.GetCurrent(new SQLite.Net.Platform.Generic.SQLitePlatformGeneric(), databasePath);

                // Vérification qu'il n'y a pas eu d'erreur à l'initialisation de la connexion
                if (!conn.Errors.IsEmpty)
                {
                    result = new TResult();
                    await ManageError(result, conn.Errors);
                    return result;
                }

                // Initialisation de la base de données
                await conn.InitializeDatabase();

                // Vérification qu'il n'y a pas eu d'erreur à l'initialisation de la base de données
                if (!conn.Errors.IsEmpty)
                {
                    result = new TResult();
                    await ManageError(result, conn.Errors);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (result == null) result = new TResult();
                await ManageError(result, ex);
                return result;
            }

            // Exécution de l'opération
            try
            {
                result = await operation.Invoke(conn, parameter, parameters);
            }
            catch (Exception ex)
            {
                if (result == null) result = new TResult();
                await ManageError(result, ex);
                return result;
            }

            return result;
        }


        internal static async Task ManageError(OperationResult result,
                                               Exception ex,
                                               EventLogEntryType logType = EventLogEntryType.Error)
        {
            await ManageError(result, ex.Message, ex.ToString(), logType: logType, moduleName: ex.Source, addLog: false);
            await Core.Tools.Log.LogException(ex);
        }

        internal static async Task ManageError(OperationResult result,
                                               ErrorsList errors,
                                               EventLogEntryType logType = EventLogEntryType.Error,
                                               [CallerMemberName] string moduleName = null)
        {
            foreach (Error e in errors)
            {
                await ManageError(result, e.Message, (e.Exception == null) ? "" : e.Exception.ToString(), logType, moduleName: moduleName);
            }
        }
        internal static async Task ManageError(OperationResult result,
                                               string message,
                                               string detailedMessage = "",
                                               EventLogEntryType logType = EventLogEntryType.Error,
                                               int errorCode = 0,
                                               [CallerMemberName] string moduleName = null,
                                               bool addLog = true)
        {
            // Ajout de l'erreur au résultat renvoyé au client
            switch (logType)
            {
                case EventLogEntryType.Error:
                case EventLogEntryType.FailureAudit:
                    result.Success = false;
                    result.Errors.Add(new Error(message, Core.Enums.ErrorType.Message));
                    break;
                case EventLogEntryType.Warning:
                    result.Success = false;
                    result.Warnings.Add(new Error(message, Core.Enums.ErrorType.Message));
                    break;
                default:
                    // On ne renvoie pas l'information au client, c'est normal
                    break;
            }

            // Ajout de l'erreur dans le log
            if (addLog)
            {
                if (detailedMessage == "") detailedMessage = message;
                await Core.Tools.Log.LogMessage(detailedMessage, (Core.Tools.Log.EventLogEntryType)logType, errorCode, moduleName);
            }
        }



        internal static bool ReadIntURIParameter(string paramName, out int paramValue)
        {
            paramValue = 0;
            string strValue;

            if (!ReadURIParameter(paramName, out strValue))
                return false;

            if (!string.IsNullOrEmpty(strValue))
                return int.TryParse(strValue, out paramValue);

            return false;
        }
        internal static bool ReadURIParameter<T>(string paramName, out T paramValue)
        {
            paramValue = default(T);
            string strValue;

            if (!ReadURIParameter(paramName, out strValue))
                return false;

            if (!string.IsNullOrEmpty(strValue))
            {
                try
                {
                    paramValue = Deserialize<T>(strValue);
                    return true;
                }
                catch { }
            }

            return false;
        }
        private static bool ReadURIParameter(string paramName, out string paramValue)
        {
            paramValue = "";
            WebOperationContext context = WebOperationContext.Current;
            if (context == null) return false;

            UriTemplateMatch uriMatch = context.IncomingRequest.UriTemplateMatch;
            if (uriMatch == null) return false;

            paramValue = uriMatch.QueryParameters[paramName];

            return true;
        }
        private static T Deserialize<T>(string rawXml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(rawXml)))
            {
                DataContractSerializer formatter0 = new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }

    }
}
