using System.Runtime.CompilerServices;
using UnityEngine;

namespace SoraTehk.Diagnostic {
    // TODO: Stub implementation atm 
    public class UnitySorLogger : ISorLogger {
        private readonly IuLogger m_Logger;

        public UnitySorLogger() {
            m_Logger = Debug.unityLogger;
        }
        protected UnitySorLogger(IuLogger logger) {
            m_Logger = logger;
        }

        public void LogTrace(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            LogType logType = LogType.Log;
            if (!m_Logger.IsLogTypeAllowed(logType)) return;

            m_Logger.Log(logType, (object)message.ToString(), context as uObject);
        }
        public void LogInfo(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            LogType logType = LogType.Log;
            if (!m_Logger.IsLogTypeAllowed(logType)) return;

            m_Logger.Log(logType, (object)message.ToString(), context as uObject);
        }
        public void LogWarning(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            LogType logType = LogType.Warning;
            if (!m_Logger.IsLogTypeAllowed(logType)) return;

            m_Logger.Log(logType, (object)message.ToString(), context as uObject);
        }
        public void LogError(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            LogType logType = LogType.Error;
            if (!m_Logger.IsLogTypeAllowed(logType)) return;

            m_Logger.Log(logType, (object)message.ToString(), context as uObject);
        }
    }
    public class UnitySorLogger<T> : UnitySorLogger, ISorLogger<T> {
        public UnitySorLogger() : base(Debug.unityLogger) { }
    }
}