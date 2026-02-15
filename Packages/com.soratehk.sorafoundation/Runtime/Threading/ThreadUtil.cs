using System.Threading;
using SoraTehk.Attributes;

namespace SoraFoundation.Threading {
    [PreloadStatic]
    public static class ThreadUtil {
        static ThreadUtil() {
            MAIN_THREAD_ID = Thread.CurrentThread.ManagedThreadId;
        }

        public static int MAIN_THREAD_ID { get; }

        public static bool G_IsMainThread() => Thread.CurrentThread.ManagedThreadId == MAIN_THREAD_ID;
    }
}