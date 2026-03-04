using System.Runtime.CompilerServices;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class TransformX {
        public static bool TryFindFirstComponent<T>(this Transform transform, out T? result, string? gameObjName = null)
            where T : Component {
            //
            result = null;
            bool checkName = !string.IsNullOrEmpty(gameObjName);
            
            if (checkName) {
                T[] cpns = transform.GetComponentsInChildren<T>(true);
                foreach (var c in cpns) {
                    if (c.gameObject.name != gameObjName) continue;
                    
                    result = c;
                    return true;
                }
            }
            else {
                result = transform.GetComponentInChildren<T>(true);
                if (result != null) {
                    return true;
                }
            }
            
            return false;
        }
    }
}