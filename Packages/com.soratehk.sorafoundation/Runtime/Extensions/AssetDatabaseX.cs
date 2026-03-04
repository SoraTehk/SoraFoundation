using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.Extensions {
    public static partial class AssetDatabaseX {
#if UNITY_EDITOR
        // TODO: Replace E_TryFindFirstAsset with E_TryAssignFirstAsset when "this ref" possible
        public static bool E_TryFindFirstAsset<T>(
            [NotNullWhen(true)] out T? result,
            string? assetName = null
        ) where T : uObject {
            result = null;

            // Special handling for Component types (search in Prefabs)
            if (typeof(Component).IsAssignableFrom(typeof(T))) {
                GUID[] guids = AssetDatabase.FindAssetGUIDs("t:Prefab");
                foreach (GUID guid in guids) {
                    // Check name
                    if (!string.IsNullOrEmpty(assetName)) {
                        string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                        if (string.IsNullOrEmpty(fileName)) continue;
                        if (!fileName.Equals(assetName)) continue;
                    }

                    var prefab = AssetDatabase.LoadAssetByGUID<GameObject>(guid);
                    if (prefab == null) continue;

                    if (prefab.TryGetComponent(typeof(T), out var component)) {
                        result = component as T;
                        if (result != null)
                            return true;
                    }
                }
            }
            // Normal uObject search
            else {
                GUID[] guids = AssetDatabase.FindAssetGUIDs($"t:{typeof(T)}");
                foreach (GUID guid in guids) {
                    // Check name
                    if (!string.IsNullOrEmpty(assetName)) {
                        string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                        if (string.IsNullOrEmpty(fileName)) continue;
                        if (!fileName.Equals(assetName)) continue;
                    }

                    result = AssetDatabase.LoadAssetByGUID<T>(guid);
                    if (result != null)
                        return true;
                }
            }

            return false;
        }
#endif
    }
}