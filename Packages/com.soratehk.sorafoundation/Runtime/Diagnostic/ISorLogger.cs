using System.Runtime.CompilerServices;

namespace SoraTehk.Diagnostic {
    public interface ISorLogger {
        public void LogTrace(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        );
        public void LogInfo(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        );
        public void LogWarning(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        );
        public void LogError(
            [InterpolatedStringHandlerArgument] ref SorLogInterpolatedStringHandler message,
            object? context = null,
            [CallerMemberName] string? memberName = null,
            [CallerFilePath] string? filePath = null,
            [CallerLineNumber] int lineNumber = 0
        );
    }
    // ReSharper disable once UnusedTypeParameter
    public interface ISorLogger<T> : ISorLogger { }
}