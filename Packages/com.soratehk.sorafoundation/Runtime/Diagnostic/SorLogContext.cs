using SoraFoundation.Threading;
using UnityEngine;

namespace SoraTehk.Diagnostic {
    public record SorLogContext {
        // TODO: Find a optimized way for background threads too
        // Time.frameCount if we on main-thread else -1
        public int SequenceCount { get; init; } = -1;
        // typeof(T), object/UObject
        public object? InstanceContext { get; init; }
        // TODO: Find a optimized way for C# object too
        // 1. (InstanceContext as UObject).GetInstanceId()
        // 2. InstanceContext.GetHashCode()
        // 3. -1
        public int InstanceId { get; init; } = -1;

        public static SorLogContext CreateBy(object? context) {
            if (ThreadUtil.G_IsMainThread()) {
                return new SorLogContext() {
                    SequenceCount = Time.frameCount,
                    InstanceContext = context,
                    InstanceId = context is uObject uObjCtx
                        ? uObjCtx.GetInstanceID()
                        : context?.GetHashCode() ?? -1
                };
            }
            else {
                return new SorLogContext() {
                    InstanceContext = context,
                    // @formatter:off TODO: There is a bug that make '??' ignore object initializer wrapping style
                    InstanceId = context?.GetHashCode() ?? -1
                    // @formatter:on
                };
            }
        }
    }
}