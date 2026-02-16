using System;
using SoraTehk.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoraTehk.Tools {
    public static partial class SoraTehkMenuItem {
        [MenuItem("Tools/SoraTehk/SetDirtyAllAssets")]
        public static void SetDirtyAllAssets() {
            GUID[] allGuids = AssetDatabase.FindAssetGUIDs("t:Object"); // UnityEngine.Object
            int skipCount = 0;

            try {
                for (int i = 0; i < allGuids.Length; i++) {
                    var guid = allGuids[i];
                    var asset = AssetDatabase.LoadAssetByGUID<uObject>(guid) ?? null;
                    if (asset == null) continue;
                    // Progress bar
                    if (i % 100 == 0) {
                        float percent = (float)i / allGuids.Length;
                        EditorUtility.DisplayProgressBar(
                            $"Set All Dirty {percent:P1}",
                            $"Processing {i}/{allGuids.Length}: {asset.name}",
                            percent
                        );
                    }
                    // Skip read-only asset
                    if (!AssetDatabase.IsOpenForEdit(asset)) {
                        skipCount++;
                        continue;
                    }
                    // Type switch
                    Type mainType = AssetDatabase.GetMainAssetTypeFromGUID(guid);
                    if (mainType == typeof(GameObject)) {
                        if (asset is GameObject gObj && gObj.IsValidOrWarn()) {
                            EditorUtility.SetDirty(gObj);
                            if (PrefabUtility.IsPartOfPrefabAsset(gObj)) {
                                try {
                                    PrefabUtility.SavePrefabAsset(gObj);
                                }
                                catch (Exception ex) {
                                    Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(gObj)}' (Prefab): {ex}");
                                    skipCount++;
                                }
                            }
                        }
                    }
                    else if (mainType == typeof(SceneAsset)) {
                        try {
                            Scene scene = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(asset), OpenSceneMode.Single);
                            foreach (var root in scene.GetRootGameObjects()) {
                                EditorUtility.SetDirty(root);
                            }

                            EditorSceneManager.MarkSceneDirty(scene);
                            EditorSceneManager.SaveScene(scene);
                        }
                        catch (Exception ex) {
                            Debug.LogWarning($"Skipping '{AssetDatabase.GetAssetPath(asset)}' (SceneAsset): {ex}");
                            skipCount++;
                        }
                    }
                    else {
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Finished: Total={allGuids.Length} , Skipped={skipCount}");
        }
    }
}