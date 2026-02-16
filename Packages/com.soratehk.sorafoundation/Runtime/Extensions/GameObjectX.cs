using UnityEditor;
using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class GameObjectX {
        public static bool IsValidOrWarn(this GameObject gObj) {
            bool isInvalid = false;

            var cpns = gObj.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var cpn in cpns) {
                if (cpn != null) continue;

                string gObjInfo;
#if UNITY_EDITOR
                if (PrefabUtility.IsPartOfPrefabAsset(gObj)) {
                    gObjInfo = AssetDatabase.GetAssetPath(gObj);
                }
                else {
                    gObjInfo = $"{gObj.name}({gObj.GetInstanceID()})";
                }
#else
                gObjInfo = $"{gObj.name}({gObj.GetInstanceID()})";
#endif
                Debug.LogWarning($"{gObjInfo} has invalid MonoBehaviour (null)", gObj);
                isInvalid = true;
            }
            
            return !isInvalid;
        }
    }
}