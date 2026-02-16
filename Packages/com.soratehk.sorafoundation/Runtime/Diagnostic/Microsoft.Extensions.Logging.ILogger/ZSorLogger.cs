#if USE_ZLOGGER
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace SoraTehk.Diagnostic {
    public static class ZSorLoggerGlobal {
        internal static readonly ILoggerFactory G_FACTORY = LoggerFactory.Create(builder => {
                builder.AddZLoggerLogProcessor(options => {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        options.CaptureThreadInfo = true;
#endif
                        return new ZLogAsyncLogProcessor();
                    }
                );
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                builder.SetMinimumLevel(LogLevel.Trace);
#else
                builder.SetMinimumLevel(LogLevel.Error);
#endif
            }
        );
        internal static readonly ILogger G_LOGGER = G_FACTORY.CreateLogger(Assembly.GetExecutingAssembly().GetName().Name);
    }
    public class ZSorLogger : ISorLogger {
        private readonly ILogger m_Logger;

        public ZSorLogger() {
            m_Logger = ZSorLoggerGlobal.G_LOGGER;
        }
        protected ZSorLogger(ILogger logger) {
            m_Logger = logger;
        }

        public void LogTrace(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            m_Logger.ZLog(
                LogLevel.Trace,
                new EventId(),
                null,
                ref message.InnerHandler,
                SorLogContext.CreateBy(context),
                memberName,
                filePath,
                lineNumber
            );
        }
        public void LogInfo(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            m_Logger.ZLog(
                LogLevel.Information,
                new EventId(),
                null,
                ref message.InnerHandler,
                SorLogContext.CreateBy(context),
                memberName,
                filePath,
                lineNumber
            );
        }
        public void LogWarning(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            m_Logger.ZLog(
                LogLevel.Warning,
                new EventId(),
                null,
                ref message.InnerHandler,
                SorLogContext.CreateBy(context),
                memberName,
                filePath,
                lineNumber
            );
        }
        public void LogError(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        ) {
            m_Logger.ZLog(
                LogLevel.Error,
                new EventId(),
                null,
                ref message.InnerHandler,
                SorLogContext.CreateBy(context),
                memberName,
                filePath,
                lineNumber
            );
        }
    }
    public class ZSorLogger<T> : ZSorLogger, ISorLogger<T> {
        public ZSorLogger() : base(ZSorLoggerGlobal.G_FACTORY.CreateLogger<T>()) { }
    }
}
#endif