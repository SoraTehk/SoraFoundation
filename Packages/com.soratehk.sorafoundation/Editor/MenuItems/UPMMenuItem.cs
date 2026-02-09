using System.Linq;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace SoraTehk.Tools {
    public static partial class SoraTehkMenuItem {
        [MenuItem("Tools/SoraTehk/RefreshUPM")]
        // ReSharper disable once Unity.IncorrectMethodSignature
        public static async Awaitable RefreshUnityPackageManager() {
            try {
                string dummyPackageId = "com.unity.collab-proxy";

                EditorUtility.DisplayProgressBar("Refreshing UPM", "Checking dummy package installation", 0.1f);
                var listReq = Client.List(true);
                await listReq.WaitForCompletion();
                EditorUtility.ClearProgressBar();

                bool dummyPackageInstalled = listReq.Result.Any(p => p.name == dummyPackageId);

                if (dummyPackageInstalled) {
                    Client.Remove(dummyPackageId);
                }
                else {
                    Client.Add(dummyPackageId);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}