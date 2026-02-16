using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if USE_ZLOGGER
using Microsoft.Extensions.Logging;
using ZLogger;

#else
using System.Text;
#endif

namespace SoraTehk.Diagnostic {
    // Based on ZLogger's facade interpolated string handlers
    [InterpolatedStringHandler]
    public ref struct SorLogInterpolatedStringHandler {
#if USE_ZLOGGER
        public ZLoggerInterpolatedStringHandler InnerHandler;
#else
        private readonly StringBuilder m_StringBuilder;
#endif
        public SorLogInterpolatedStringHandler(
            int literalLength,
            int formattedCount,
            out bool enabled
        ) { //
#if USE_ZLOGGER
            InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, ZSorLoggerGlobal.G_LOGGER, LogLevel.Debug, out enabled);
#else
            m_StringBuilder = new StringBuilder(literalLength);
            enabled = true;
#endif
        }

        public void AppendLiteral([ConstantExpected] string s) {
#if USE_ZLOGGER
            InnerHandler.AppendLiteral(s);
#else
            m_StringBuilder.Append(s);
#endif
        }

        public void AppendFormatted<T>(
            T value,
            int alignment = 0,
            string? format = null,
            [CallerArgumentExpression("value")] string? argumentName = null
        ) { //
#if USE_ZLOGGER
            InnerHandler.AppendFormatted(value, alignment, format, argumentName);
#else
            var formatString = $"{alignment}:{format}";
            m_StringBuilder.Append(string.Format($"{{0,{formatString}}}", value));
#endif
        }
        public void AppendFormatted<T>(
            T? value,
            int alignment = 0,
            string? format = null,
            [CallerArgumentExpression("value")] string? argumentName = null
        ) where T : struct {
            //
#if USE_ZLOGGER
            InnerHandler.AppendFormatted(value, alignment, format, argumentName);
#else
            var formatString = $"{alignment}:{format}";
            m_StringBuilder.Append(string.Format($"{{0,{formatString}}}", value));
#endif
        }
        public void AppendFormatted<T>(
            (string, T) namedValue,
            int alignment = 0,
            string? format = null,
            string? _ = null
        ) { //
#if USE_ZLOGGER
            // ReSharper disable once RedundantCallerArgumentExpressionDefaultValue
            InnerHandler.AppendFormatted(namedValue, alignment, format, nameof(namedValue));
#else
            var formatString = $"{alignment}:{format}";
            m_StringBuilder.Append(string.Format($"{{0,{formatString}}}", namedValue.Item2));
#endif
        }

        public override string ToString() {
#if USE_ZLOGGER
            return InnerHandler.GetState().ToString();
#else
            return m_StringBuilder.ToString();
#endif
        }
    }
}