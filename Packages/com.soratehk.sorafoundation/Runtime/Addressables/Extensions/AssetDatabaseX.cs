#if USE_ADDRESSABLE
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SoraTehk.AddressablesAddons;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoraTehk.Extensions {
    public static partial class AssetDatabaseX {
#if UNITY_EDITOR
        // TODO: Replace E_TryFindFirstAsset with E_TryAssignFirstAsset when "this ref" possible
        public static bool E_TryFindFirstAsset<T>(
            [NotNullWhen(true)] out AssetReferenceComponent<T>? assetRef,
            string? assetName = null
        ) where T : Component {
            //
            assetRef = null;

            GUID[] guids = AssetDatabase.FindAssetGUIDs("t:Prefab");
            foreach (GUID guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Equals(assetName)) continue;
                }

                // Check matching asset
                var prefab = AssetDatabase.LoadAssetByGUID<GameObject>(guid);
                if (prefab.TryGetComponent(out T _)) {
                    assetRef = new AssetReferenceComponent<T>();
                    assetRef.SetEditorAsset(prefab);
                    return true;
                }
            }

            return false;
        }
        public static bool E_TryFindFirstAsset<T>(
            [NotNullWhen(true)] out AssetReferenceuObject<T>? assetRef,
            string? assetName = null
        ) where T : uObject {
            //
            assetRef = null;

            GUID[] guids = AssetDatabase.FindAssetGUIDs($"t:{typeof(T)}");
            foreach (var guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Equals(assetName)) continue;
                }

                assetRef = new AssetReferenceuObject<T>();
                assetRef.SetEditorAsset(AssetDatabase.LoadAssetByGUID<T>(guid));
                return true;
            }

            return false;
        }
        public static bool E_TryFindFirstAsset(
            out AssetReferenceScene? assetRef,
            string? assetName = null
        ) { //
            assetRef = null;

            GUID[] guids = AssetDatabase.FindAssetGUIDs($"t:SceneAsset");
            foreach (var guid in guids) {
                // Check name
                if (!string.IsNullOrEmpty(assetName)) {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(guid));
                    if (string.IsNullOrEmpty(fileName)) continue;
                    if (!fileName.Equals(assetName)) continue;
                }

                assetRef = new AssetReferenceScene();
                assetRef.SetEditorAsset(AssetDatabase.LoadAssetByGUID<SceneAsset>(guid));
                return true;
            }

            return false;
        }
#endif
    }
}
#endif