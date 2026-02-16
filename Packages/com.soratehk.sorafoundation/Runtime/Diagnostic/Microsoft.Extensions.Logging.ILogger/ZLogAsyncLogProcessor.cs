#if USE_ZLOGGER
using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using SoraTehk.Extensions;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;

namespace SoraTehk.Diagnostic {
    public class ZLogAsyncLogProcessor : IAsyncLogProcessor {
        [ThreadStatic] private static ArrayBufferWriter<byte>? gBufferWriter;
        private readonly IZLoggerFormatter m_Formatter = new PlainTextZLoggerFormatter();

        [HideInCallstack]
        public void Post(IZLoggerEntry entry) {
            try {
                gBufferWriter ??= new ArrayBufferWriter<byte>();
                gBufferWriter.Clear();
                m_Formatter.FormatLogEntry(gBufferWriter, entry);

                uObject? uObjCtx;

                // TODO: Fallback for non-SoraLogContext
                if (entry.LogInfo.Context is not SorLogContext ctx) {
                    var msg = Encoding.UTF8.GetString(gBufferWriter.WrittenSpan);
                    uObjCtx = entry.LogInfo.Context as uObject;
                    switch (entry.LogInfo.LogLevel) {
                        case LogLevel.Trace:
                        case LogLevel.Debug:
                        case LogLevel.Information:
                            Debug.Log(msg, uObjCtx);
                            break;
                        case LogLevel.Warning:
                            Debug.LogWarning(msg, uObjCtx);
                            break;
                        case LogLevel.Error:
                        case LogLevel.Critical:
                            Debug.LogError(msg, uObjCtx);
                            break;
                        case LogLevel.None:
                        default:
                            break;
                    }

                    return;
                }
                else {
                    uObjCtx = ctx.InstanceContext as uObject;
                }

                // Unity main-thread's frame
                int frame = ctx.SequenceCount;
                Timestamp timeStamp = entry.LogInfo.Timestamp;
                int threadId = entry.LogInfo.ThreadInfo.ThreadId;
                // Category
                ReadOnlySpan<byte> categorySpan = entry.LogInfo.Category.Utf8Span;
                // Instance
                string typeName = ctx.InstanceContext switch {
                    Type t => $"S_{t.Name}", // typeof(T), static
                    null   => "null", // null
                    _      => ctx.InstanceContext.GetType().Name // Type, this
                };
                int instanceId = ctx.InstanceId;
                string memberName = entry.LogInfo.MemberName ?? "";
                
                // Output should be [frame|time|threadId][category][type(instanceId)|member]<user log message>
                string formattedMsg;
                using (var sb = ZString.CreateUtf8StringBuilder(true)) {
                    // When & where did this happen?
                    sb.Append('[');
                    sb.Append(frame);
                    sb.Append('|');
                    sb.Append(timeStamp.Local.ToString("HH:mm:ss.fff"));
                    sb.Append('|');
                    // @formatter:off
                    sb.Append(threadId
#if UNITY_EDITOR 
                        .ToString().RTColorByHash()
#endif
                    );
                    // @formatter:on
                    sb.Append(']');
                    
                    // Category
                    sb.Append('[');
                    sb.AppendLiteral(categorySpan);
                    sb.Append(']');
                    
                    // Instance detail
                    sb.Append('[');
                    // @formatter:off
                    sb.Append(typeName
#if UNITY_EDITOR
                        .RTColorByHash()
#endif
                    );
                    // @formatter:on
                    sb.Append('(');
                    // @formatter:off
                    sb.Append(instanceId
#if UNITY_EDITOR 
                        .ToString().RTColorByHash()
#endif
                    );
                    // @formatter:on
                    sb.Append(')');
                    if (!string.IsNullOrEmpty(memberName)) {
                        sb.Append('|');
                        sb.Append(memberName);
                    }
                    sb.Append(']');

                    // Finalizing
                    sb.AppendLiteral(gBufferWriter.WrittenSpan);
                    formattedMsg = sb.ToString();
                }

                switch (entry.LogInfo.LogLevel) {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                    case LogLevel.Information:
                        Debug.Log(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.Error:
                    case LogLevel.Critical:
                        Debug.LogError(formattedMsg, uObjCtx);
                        break;
                    case LogLevel.None:
                    default:
                        break;
                }
            }
            finally {
                entry.Return();
            }
        }

        public ValueTask DisposeAsync() => default;
    }
}
#endif