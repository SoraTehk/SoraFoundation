using System.Linq;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class TransformX {
        public static bool TryFindFirstComponent<T>(this Transform transform, out T? result, string? gameObjName = null)
            where T : Component {
            //
            result = null;
            bool checkName = !string.IsNullOrEmpty(gameObjName);

            var cpns = transform.GetComponentsInChildren<T>(true);
            if (checkName) {
                foreach (var c in cpns) {
                    if (c.gameObject.name != gameObjName) continue;

                    result = c;
                    return true;
                }
            }
            else {
                result = cpns.FirstOrDefault();
                return result != null;
            }

            return false;
        }
        public static bool TryFindFirstComponentInParent<T>(this Transform transform, out T? result, string? gameObjName = null)
            where T : Component {
            //
            result = null;
            bool checkName = !string.IsNullOrEmpty(gameObjName);

            var cpns = transform.GetComponentsInParent<T>(true);
            if (checkName) {
                foreach (var c in cpns) {
                    if (c.gameObject.name == gameObjName) {
                        result = c;
                        return true;
                    }
                }
            }
            else {
                result = cpns.FirstOrDefault();
                return result != null;
            }

            return false;
        }
    }
}