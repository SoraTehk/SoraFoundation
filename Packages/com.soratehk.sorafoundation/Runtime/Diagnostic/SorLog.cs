using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SoraTehk.Attributes;
using SoraTehk.Extensions;

namespace SoraTehk.Diagnostic {
    [PreloadStatic]
    public static class SorLog {
        private static ISorLogger? m_DefaultLogger;
        private static readonly Dictionary<Type, ISorLogger> m_Type2Logger = new Dictionary<Type, ISorLogger>();

        public static ISorLogger GetLogger() {
            if (m_DefaultLogger == null) {
#if USE_ZLOGGER
                m_DefaultLogger = new ZSorLogger();
#else
                m_DefaultLogger = new UnitySorLogger();
#endif
            }

            return m_DefaultLogger;
        }
        public static ISorLogger GetLogger<T>() {
#if USE_ZLOGGER
            return m_Type2Logger.GetOrAdd(typeof(T), _ => new ZSorLogger<T>());
#else
            return m_Type2Logger.GetOrAdd(typeof(T), _ => new UnitySorLogger<T>());
#endif
        }

        public static void LogTrace(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) { //
            GetLogger().LogTrace(ref message, context, memberName, filePath, lineNumber);
        }
        public static void LogInfo(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) { //
            GetLogger().LogInfo(ref message, context, memberName, filePath, lineNumber);
        }
        public static void LogWarning(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) { //
            GetLogger().LogWarning(ref message, context, memberName, filePath, lineNumber);
        }
        public static void LogError(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) { //
            GetLogger().LogError(ref message, context, memberName, filePath, lineNumber);
        }
    }
}