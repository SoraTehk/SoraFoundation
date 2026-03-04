using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Extensions {
    public static partial class SceneX {
        public static bool TryFindFirstComponent<T>(this Scene scene, out T? result, string gameObjName = "") where T : Component {
            result = null;
            if (!scene.IsValid()) return false;

            bool checkName = !string.IsNullOrEmpty(gameObjName);

            var rootGObjs = scene.GetRootGameObjects();

            if (checkName) {
                foreach (var gObj in rootGObjs) {
                    var cpns = gObj.GetComponentsInChildren<T>(true);
                    foreach (var component in cpns) {
                        if (component.gameObject.name == gameObjName) {
                            result = component;
                            return true;
                        }
                    }
                }
            }
            else {
                foreach (var root in rootGObjs) {
                    result = root.GetComponentInChildren<T>(true);
                    if (result != null) return true;
                }
            }

            return false;
        }
    }
}