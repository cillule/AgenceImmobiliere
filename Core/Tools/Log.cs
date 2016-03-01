using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PCLStorage;
using Oyosoft.AgenceImmobiliere.Core.Exceptions;
using Oyosoft.AgenceImmobiliere.Core.ViewModels;

namespace Oyosoft.AgenceImmobiliere.Core.Tools
{
    public class Log : BaseNotifyPropertyChanged
    {

        private static Log _log = null;
        public static async Task<Log> GetCurrent()
        {
            if (_log == null)
            {
                _log = new Log();
                await _log.Initialize();
            }
            return _log;
        }

        public static async Task<string> LogException(Exception ex, int errorCode = 0, [CallerMemberName] string moduleName = null)
        {
            string error = ex.Message;
            ErrorsList errs = await Tools.Log.LogMessage(ex.ToString(), EventLogEntryType.Error, errorCode, moduleName);
            if (!errs.IsEmpty) error += "\n" + errs.ToString();
            return error;
        }

        public static async Task<string> LogSQLException(string sqlQuery, Exception ex, int errorCode = 0, [CallerMemberName] string moduleName = null)
        {
            string error = ex.Message + "\nRequête en cours d'exécution : " + sqlQuery;
            ErrorsList errs = await Tools.Log.LogMessage(ex.ToString() + "\n\nRequête en cours d'exécution : " + sqlQuery, EventLogEntryType.Error, errorCode, moduleName);
            if (!errs.IsEmpty) error += "\n" + errs.ToString();
            return error;
        }

        public static async Task<ErrorsList> LogMessage(string message,
                                                        EventLogEntryType logType = EventLogEntryType.Information,
                                                        int errorCode = 0,
                                                        [CallerMemberName] string moduleName = null)
        {
            ErrorsList errs = new ErrorsList();
            try
            {
                Log log = await Tools.Log.GetCurrent();
                await log.AddEventLog(message, logType, errorCode, moduleName);
                errs.AddRange(log.Errors);
            }
            catch (Exception ex)
            {
                errs.Add(ex.Message, Enums.ErrorType.Exception, ex);
            }
            return errs;
        }

        



        public enum EventLogEntryType
        {
            Error = 1,
            Warning = 2,
            Information = 4,
            SuccessAudit = 8,
            FailureAudit = 16
        }

        public const string LOG_FILE_NAME = "Log.log";
        
        private bool _initialized;
        private IFile _logFile;
        private DateTimeFormatInfo _dateFormat;
        private string _fileContent;
        private ErrorsList _errors;

        public ErrorsList Errors
        {
            get { return _errors; }
        }
        public string LogContent { get { return _fileContent; } }

        private Log()
        {
            this._initialized = false;
            this._errors = new ErrorsList();
        }

        private async Task<bool> Initialize()
        {
            try
            {
                this._errors.Clear();
                this._fileContent = "";
                this._dateFormat = CultureInfo.CurrentCulture.DateTimeFormat;

                IFolder repertoire = FileSystem.Current.LocalStorage;
                this._logFile = await repertoire.CreateFileAsync(LOG_FILE_NAME, CreationCollisionOption.OpenIfExists);
                SetProperty(ref _fileContent, await this._logFile.ReadAllTextAsync(), "LogContent");

                this._initialized = true;
            }
            catch (Exception ex)
            {
                this._errors.Add("Erreur pendant l'initialisation de l'objet Log :\n" + ex.Message, Enums.ErrorType.Exception, ex);
                this._initialized = false;
            }

            return this._errors.IsEmpty;
        }

        private async Task<bool> AddEventLog(string message,
                                             EventLogEntryType logType = EventLogEntryType.Information,
                                             int errorCode = 0,
                                             [CallerMemberName] string moduleName = null)
        {
            CheckIfInstanceIsInitialized();

            System.Text.StringBuilder str = new System.Text.StringBuilder();
            this._errors.Clear();

            try
            {
                str.AppendFormat("[{0,-10:G} - {1,-8:G}]", DateTime.Now.ToString(this._dateFormat.ShortDatePattern), DateTime.Now.ToString(this._dateFormat.ShortTimePattern));
                str.AppendFormat(" {0}", AssemblyInfo.AssemblyTitle);
                if (!string.IsNullOrEmpty(moduleName)) str.AppendFormat(" ({0})", moduleName);
                str.AppendFormat(" - {0}", logType.ToString());
                if (errorCode != 0) str.AppendFormat(" - Code : {0}", errorCode);

                str.Append("\r\n");
                str.Append(message);
                str.Append("\r\n");

                this._fileContent += "\n\n" + str.ToString();
                SetProperty(ref _fileContent, this._fileContent + "\n\n" + str.ToString(), "LogContent");

                await this._logFile.WriteAllTextAsync(this._fileContent);
            }
            catch (Exception ex)
            {
                this._errors.Add("Erreur pendant l'écriture d'un message dans le fichier de log :\n" + ex.Message, Enums.ErrorType.Exception, ex);
            }

            return this._errors.IsEmpty;
        }

        private void CheckIfInstanceIsInitialized()
        {
            if (!this._initialized) throw new NotInitializedException<Log>(this._errors);
        }

    }
}
