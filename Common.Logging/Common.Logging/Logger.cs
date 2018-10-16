using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Generic;

namespace Common.Logging
{
    /// <summary>
    /// Logging levels
    /// </summary>
    public enum LogLevels
    {
        /// <summary>
        /// Debug log level
        /// </summary>
        Debug,

        /// <summary>
        /// Info log level
        /// </summary>
        Info,

        /// <summary>
        /// Warn log level
        /// </summary>
        Warn,

        /// <summary>
        /// Error log level
        /// </summary>
        Error,

        /// <summary>
        /// Fatal log level
        /// </summary>
        Fatal
    }

    public interface ILogData<T>
    {
        DateTimeOffset Timestamp { get; set; }

        string CodeFile { get; set; }
        string MemberName { get; set; }
        int LineNumber { get; set; }
        string Message { get; set;}
        Exception Exception { get; set; }

        Type TypeForLogging { get; set; }

        LogLevels LogLevel { get; set; }

    }

    /// <summary>
    /// Executing singleton for thread-safe concurrent logging
    /// </summary>
    public sealed class Logger: IDisposable
    {
        #region Fields
        private static Lazy<Logger> _instance = new Lazy<Logger>();

        private GenericSpooler<LogData> _logDataSpooler;
        private static string _callFilepath;
        private static string _callerName;
        private static int _lineNo;
        #endregion

        #region Delegates and Events
        /// <summary>
        /// Event to subscribe to get a copy of Debug logs entries
        /// </summary>
        public event Action<LogData> DebugEvents;

        /// <summary>
        /// Event to subscribe to get a copy of Info logs entries
        /// </summary>
        public event Action<LogData> InfoEvents;


        /// Event to subscribe to get a copy of Warn logs entries
        public event Action<LogData> WarnEvents;

        /// <summary>
        /// Event to subscribe to get a copy of Error logs entries
        /// </summary>
        public event Action<LogData> ErrorEvents;

        /// <summary>
        /// Event to subscribe to get a copy of Fatal logs entries
        /// </summary>
        public event Action<LogData> FatalEvents;
        #endregion

        #region Properties
        private GenericSpooler<LogData> LogDataSpooler
        {
            get
            {
                _logDataSpooler = _logDataSpooler ?? new GenericSpooler<LogData>(WriteLogEntry);
                return _logDataSpooler;
            }
        }
        #endregion

        #region Singleton 
        /// <summary>
        /// Singleton Instance
        /// </summary>
        /// <param name="callerFilepath">supplied by system the filepath of the calling class file</param>
        /// <param name="callerName">supplied by the system the member name</param>
        /// <param name="linenumber">supplied by the system the linenumber in the file</param>
        /// <returns>the thread safe (Lazy) instance of the Logger</returns>
        public static Logger Instance([CallerFilePath] string callerFilepath = null, [CallerMemberName] string callerName = null, [CallerLineNumber] int linenumber = 0)
        {
            _callerName = callerName;
            _callFilepath = callerFilepath.Substring(callerFilepath.LastIndexOf("\\") + 1);
            _lineNo = linenumber;

            return _instance.Value;
        }
        #endregion

        #region Ctors and Dtors
        /// <summary>
        /// Finalizer for logger
        /// </summary>
        ~Logger()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }
        #endregion

        #region Public Debug
        /// <summary>
        /// Log a Debug message
        /// </summary>
        /// <param name="loggingType">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Debug(string message, params object[] formatParameters)
        {
            QueueLogData(LogLevels.Debug, message, formatParameters);
        }

        /// <summary>
        /// Log a debug message with an exception
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="ex">the exception for the log</param>
        public void Debug(string message, Exception ex)
        {
            QueueLogData(LogLevels.Debug, message, ex);
        }

        /// <summary>
        /// Log a debug message to a logger identified by type
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        public void Debug(Type type, string message, params object[] formatParameters)
        {
            QueueLogData(type, LogLevels.Debug, message, formatParameters);
        }

        /// <summary>
        /// Log a debug message to a logger identified by type
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message</param>
        /// <param name="ex">the exception for the log</param>
        public void Debug(Type type, string message, Exception ex)
        {
            QueueLogData(type, LogLevels.Debug, message, ex);
        }
        #endregion

        #region Public Info
        /// <summary>
        /// Log an Info message
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Info(string message, params object[] formatParameters)
        {
            QueueLogData(LogLevels.Info, message, formatParameters);
        }

        /// <summary>
        /// Log an Info message with an exception
        /// </summary>
        /// <param name="loggingType">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Info(string message, Exception ex)
        {
            QueueLogData(LogLevels.Info, message, ex);
        }

        /// <summary>
        /// Log an Info message
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Info(Type type, string message, params object[] formatParameters)
        {
            QueueLogData(type, LogLevels.Info, message, formatParameters);
        }

        /// <summary>
        /// Log an Info message with an exception
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Info(Type type, string message, Exception ex)
        {
            QueueLogData(type, LogLevels.Info, message, ex);
        }
        #endregion

        #region Public Warn
        /// <summary>
        /// Log a Warn message
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Warn(string message, params object[] formatParameters)
        {
            QueueLogData(LogLevels.Warn, message, formatParameters);
        }

        /// <summary>
        /// Log a Warn message with an exception
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Warn(string message, Exception ex)
        {
            QueueLogData(LogLevels.Warn, message, ex);
        }

        /// <summary>
        /// Log a Warn message
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Warn(Type type, string message, params object[] formatParameters)
        {
            QueueLogData(type, LogLevels.Warn, message, formatParameters);
        }

        /// <summary>
        /// Log a Warn message with an exception
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Warn(Type type, string message, Exception ex)
        {
            QueueLogData(type, LogLevels.Warn, message, ex);
        }

        #endregion

        #region Public Error
        /// <summary>
        /// Log an Error message
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Error(string message, params object[] formatParameters)
        {
            QueueLogData(LogLevels.Error, message, formatParameters);
        }

        /// <summary>
        /// Log an Error message with an exception
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Error(string message, Exception ex)
        {
            QueueLogData(LogLevels.Error, message, ex);
        }

        /// <summary>
        /// Log an Error message
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Error(Type type, string message, params object[] formatParameters)
        {
            QueueLogData(type, LogLevels.Error, message, formatParameters);
        }

        /// <summary>
        /// Log an Error message with an exception
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Error(Type type, string message, Exception ex)
        {
            QueueLogData(type, LogLevels.Error, message, ex);
        }

        #endregion

        #region Public Fatal
        /// <summary>
        /// Log a Fatal message
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Fatal(string message, params object[] formatParameters)
        {
            QueueLogData(LogLevels.Fatal, message, formatParameters);
        }

        /// <summary>
        /// Log a Fatal message with an exception
        /// </summary>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Fatal(string message, Exception ex)
        {
            QueueLogData(LogLevels.Fatal, message, ex);
        }

        /// <summary>
        /// Log a Fatal message
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="formatParameters">if message is string format, the objects for that format</param>
        /// <exception cref="FormatException">thrown if the message and the format parameter do not collate</exception>
        /// <exception cref="ArgumentNullException">the loggingType or the message is null</exception>
        public void Fatal(Type type, string message, params object[] formatParameters)
        {
            QueueLogData(type, LogLevels.Fatal, message, formatParameters);
        }

        /// <summary>
        /// Log a Fatal message with an exception
        /// </summary>
        /// <param name="type">the calling class type</param>
        /// <param name="message">the message (optionally can be a string pattern for string.format)</param>
        /// <param name="ex">the exception for the log</param>
        public void Fatal(Type type, string message, Exception ex)
        {
            QueueLogData(type, LogLevels.Fatal, message, ex);
        }

        #endregion

        #region Privates
        private string GetFullExceptionMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Exception: {0} type {1} = {1} StackTrace: {2}", ex.Message, ex.GetType(), ex.StackTrace);
            if(ex.InnerException != null)
            {
                sb.AppendLine("-" + GetFullExceptionMessage(ex.InnerException));
            }

            return sb.ToString();
        }

        private void WriteLogEntry(LogData logData)
        {

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore
            };

            log4net.ILog log = log4net.LogManager.GetLogger("Logger");
            if(logData.TypeForLogging != null)
            {
                log = log4net.LogManager.GetLogger(logData.TypeForLogging);
            }

            switch (logData.LogLevel)
            {
                case LogLevels.Debug:
                    log.Debug(logData);
                    RaiseDebugEvent(logData);
                    break;
                case LogLevels.Info:
                    log.Info(logData);
                    RaiseInfoEvent(logData);
                    break;
                case LogLevels.Warn:
                    log.Warn(logData);
                    RaiseWarnEvent(logData);
                    break;
                case LogLevels.Error:
                    log.Error(logData);
                    RaiseErrorEvent(logData);
                    break;
                case LogLevels.Fatal:
                    log.Fatal(logData);
                    RaiseFatalEvent(logData);
                    break;
            }
        }
        #endregion

        #region Private Queuing to Spooler

        private void QueueLogData(LogLevels logLevel, string message, params object[] formatParameters)
        {
            var formattedMessage = message;
            if (formatParameters != null && formatParameters.Count() > 0)
            {
                formattedMessage = string.Format(message, formatParameters);
            }

            LogData logData = new LogData()
            {
                Timestamp = DateTimeOffset.Now,
                Message = message,
                LogLevel = logLevel,
                CodeFile = _callFilepath,
                MemberName = _callerName,
                LineNumber = _lineNo
            };

            LogDataSpooler.AddItem(logData);
        }

        private void QueueLogData(Type loggingType, LogLevels logLevel, string message, params object[] formatParameters)
        {
            var formattedMessage = message;
            if (formatParameters != null && formatParameters.Count() > 0)
            {
                formattedMessage = string.Format(message, formatParameters);
            }

            LogData logData = new LogData()
            {
                Timestamp = DateTimeOffset.Now,
                Message = message,
                LogLevel = logLevel,
                CodeFile = _callFilepath,
                MemberName = _callerName,
                LineNumber = _lineNo,
                TypeForLogging = loggingType
            };

            LogDataSpooler.AddItem(logData);
        }

        private void QueueLogData(LogLevels logLevel, string message, Exception ex)
        {
            var formattedMessage = message;

            if (ex != null && !string.IsNullOrEmpty(ex.Message) && !string.IsNullOrEmpty(ex.StackTrace))
            {
                formattedMessage = GetFullExceptionMessage(ex);
            }

            LogData logData = new LogData()
            {
                Timestamp = DateTimeOffset.Now,
                Message = message,
                LogLevel = logLevel,
                CodeFile = _callFilepath,
                MemberName = _callerName,
                LineNumber = _lineNo,
                Exception = ex
            };

            LogDataSpooler.AddItem(logData);
        }

        private void QueueLogData(Type loggingType, LogLevels logLevel, string message, Exception ex)
        {
            var formattedMessage = message;

            if (ex != null && !string.IsNullOrEmpty(ex.Message) && !string.IsNullOrEmpty(ex.StackTrace))
            {
                formattedMessage = GetFullExceptionMessage(ex);
            }

            LogData logData = new LogData()
            {
                Timestamp = DateTimeOffset.Now,
                Message = message,
                LogLevel = logLevel,
                CodeFile = _callFilepath,
                MemberName = _callerName,
                LineNumber = _lineNo,
                TypeForLogging = loggingType,
                Exception = ex
            };

            LogDataSpooler.AddItem(logData);
        }

        #endregion

        #region Private LifeCycle
        private void Close()
        {
            if (_logDataSpooler != null)
            {
                _logDataSpooler.Dispose();
            }
        }
        #endregion

        #region Raising Events
        private void RaiseDebugEvent(LogData toBeLogged)
        {
            DebugEvents?.Invoke(toBeLogged);
        }

        private void RaiseInfoEvent(LogData toBeLogged)
        {
            InfoEvents?.Invoke(toBeLogged);
        }

        private void RaiseWarnEvent(LogData toBeLogged)
        {
            WarnEvents?.Invoke(toBeLogged);
        }

        private void RaiseErrorEvent(LogData toBeLogged)
        {
            ErrorEvents?.Invoke(toBeLogged);
        }

        private void RaiseFatalEvent(LogData toBeLogged)
        {
            FatalEvents?.Invoke(toBeLogged);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                Close();

                disposedValue = true;
            }
        }


        /// <summary>
        /// Dispose the logger instance
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
