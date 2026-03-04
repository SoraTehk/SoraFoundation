using NUnit.Framework;
using SoraTehk.Diagnostic;
using UnityEngine.SceneManagement;

namespace SoraTehk.Tests {
    public class GeneralTests {
        [Test]
        public void DefaultLogInfo() {
            var ctx = SceneManager.GetActiveScene().GetRootGameObjects()[0];
            SorLog.LogInfo($"Lorem ipsum", ctx);
        }

        [Test]
        public void LoggerLogInfo() {
            var ctx = SceneManager.GetActiveScene().GetRootGameObjects()[0];
            ISorLogger logger = SorLog.GetLogger<SorLogContext>();
            logger.LogInfo($"Lorem ipsum", ctx);
        }
    }
}