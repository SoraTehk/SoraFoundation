#if USE_ZLOGGER
global using ILogger = Microsoft.Extensions.Logging.ILogger;
#endif
global using IuLogger = UnityEngine.ILogger;
global using Object = System.Object;
global using uObject = UnityEngine.Object;

// Editor namespace to avoid "using" compiler error
namespace SoraTehk.Prepare {
    internal class NamespaceHolder { }
}